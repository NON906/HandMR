using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

namespace HandMR
{
    public class HandMRInputModule : StandaloneInputModule
    {
        public HandVRSphereHand[] Hands;
        public float TouchDistance = 0.02f;
        public bool GrabDetect = true;
        public float LeaveTime = 0.5f;

        HandMRManager handMRManager_ = null;
        List<Collider> colliders_ = new List<Collider>();
        GameObject prevDetectObj_ = null;
        float prevDetectTime_ = 0f;
        bool isGrabDetected_ = false;
        Vector2 lastPosition_;
        Vector2 startDragPosition_;

        PointerEventData submitPointerData_ = null;

        bool isOnScreen(Selectable selectable)
        {
            Canvas canvas = selectable.GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                return false;
            }

            return canvas.renderMode != RenderMode.WorldSpace;
        }

        void addColliderToSelectable()
        {
            Selectable[] selectables = Selectable.allSelectablesArray;
            int uiLayer = LayerMask.NameToLayer("UI");

            foreach (Selectable selectable in selectables)
            {
                if (!isOnScreen(selectable) && selectable.GetComponent<Collider>() == null)
                {
                    selectable.gameObject.layer = uiLayer;

                    BoxCollider collider = selectable.gameObject.AddComponent<BoxCollider>();
                    collider.isTrigger = true;
                    Rect rect = selectable.GetComponent<RectTransform>().rect;
                    float sizeZ = rect.width < rect.height ? rect.width : rect.height;
                    collider.center = new Vector3(rect.x + rect.width * 0.5f, rect.y + rect.height * 0.5f, sizeZ * 0.5f);
                    collider.size = new Vector3(rect.width, rect.height, sizeZ);

                    colliders_.Add(collider);
                }
            }
        }

        void changeEnabledSelectable(bool onScreenFlag, Selectable[] selectables)
        {
            foreach (Selectable selectable in selectables)
            {
                if (isOnScreen(selectable))
                {
                    selectable.enabled = onScreenFlag;
                }
                else
                {
                    selectable.enabled = !onScreenFlag;
                }
            }
        }

        void reEnabledSelectable(Selectable[] selectables)
        {
            foreach (Selectable selectable in selectables)
            {
                selectable.enabled = true;
            }
        }

        void resetSelect()
        {
            eventSystem.SetSelectedGameObject(null);
        }

        Vector2 pointerDataPosition(GameObject detectObject, Vector3 touchPosition)
        {
            Vector3 localPos = detectObject.transform.InverseTransformPoint(touchPosition);
            localPos.z = 0f;
            Vector3 worldPos = detectObject.transform.TransformPoint(localPos);
            return worldPos;
        }

        void uiDetectControl(GameObject detectObject, Vector3 touchPosition, bool grab)
        {
            if (prevDetectObj_ != detectObject)
            {
                if (eventSystem.currentSelectedGameObject != detectObject)
                {
                    eventSystem.SetSelectedGameObject(detectObject);
                }

                if (grab && isGrabDetected_)
                {
                    return;
                }

                PointerEventData pointerData = new PointerEventData(eventSystem);
                Canvas canvas = detectObject.GetComponentInParent<Canvas>();
                Camera camera = Camera.main;
                if (canvas != null && canvas.worldCamera != null)
                {
                    camera = canvas.worldCamera;
                }
                pointerData.position = pointerDataPosition(detectObject, touchPosition);
                pointerData.pressPosition = pointerData.position;

                GameObject submitObj = ExecuteEvents.GetEventHandler<ISubmitHandler>(detectObject);
                if (submitObj != null)
                {
                    if (grab || submitObj.GetComponent<Dropdown>() == null)
                    {
                        ExecuteEvents.Execute(submitObj, pointerData, ExecuteEvents.submitHandler);
                        addColliderToSelectable();
                        submitPointerData_ = null;
                    }
                    else
                    {
                        submitPointerData_ = pointerData;
                    }
                }

                pointerData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(detectObject);
                if (pointerData.pointerDrag != null)
                {
                    ExecuteEvents.Execute(pointerData.pointerDrag, pointerData, ExecuteEvents.initializePotentialDrag);
                    lastPosition_ = pointerData.position;
                    startDragPosition_ = pointerData.pressPosition;
                }

                prevDetectObj_ = detectObject;
                prevDetectTime_ = Time.time;
                isGrabDetected_ = true;
            }
            else
            {
                PointerEventData pointerData = new PointerEventData(eventSystem);
                pointerData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(detectObject);
                if (pointerData.pointerDrag != null)
                {
                    Canvas canvas = detectObject.GetComponentInParent<Canvas>();
                    Camera camera = Camera.main;
                    if (canvas != null && canvas.worldCamera != null)
                    {
                        camera = canvas.worldCamera;
                    }
                    pointerData.position = pointerDataPosition(detectObject, touchPosition);
                    pointerData.delta = pointerData.position - lastPosition_;
                    pointerData.dragging = true;
                    pointerData.pressPosition = startDragPosition_;
                    ExecuteEvents.Execute(pointerData.pointerDrag, pointerData, ExecuteEvents.dragHandler);
                    lastPosition_ = pointerData.position;
                }

                prevDetectTime_ = Time.time;
                isGrabDetected_ = true;
            }
        }

        public override void Process()
        {
            if (handMRManager_ == null)
            {
                handMRManager_ = FindObjectOfType<HandMRManager>();
            }
            if (colliders_.Count <= 0)
            {
                addColliderToSelectable();
            }

            bool noHands = true;
            foreach (var hand in Hands)
            {
                if (hand.IsTrackingHand)
                {
                    noHands = false;
                    break;
                }
            }
            if (noHands)
            {
                if (Time.time - prevDetectTime_ > LeaveTime)
                {
                    Selectable[] selectables2 = Selectable.allSelectablesArray;
                    changeEnabledSelectable(true, selectables2);
                    base.Process();
                    reEnabledSelectable(selectables2);
                }
                return;
            }

            Transform cameraTrans = handMRManager_.GetCameraTransform();
            List<RaycastHit> tempHits = new List<RaycastHit>();

            bool handIsOpened = false;
            foreach (var hand in Hands)
            {
                if (!hand.IsTrackingHand)
                {
                    continue;
                }

                bool grabed = false;
                if (GrabDetect)
                {
#if false
                    grabed = true;
                    bool opened = true;
                    for (int loop = 1; loop < 5; loop++)
                    {
                        if (hand.GetFingerOpened(loop))
                        {
                            grabed = false;
                        }
                        else
                        {
                            opened = false;
                        }
                    }
                    if (Vector3.Distance(hand.GetFinger(4).position, hand.GetFinger(8).position) < 0.03f)
                    {
                        grabed = true;
                        opened = false;
                    }
                    if (opened)
                    {
                        handIsOpened = true;
                    }
#endif
                    grabed = hand.IsGrab;
                    handIsOpened = !grabed;
                }

                Vector3 forward = (hand.GetFinger(8).position - cameraTrans.position).normalized;

                RaycastHit[] hits = Physics.RaycastAll(cameraTrans.position, forward,
                    Mathf.Infinity, 1 << LayerMask.NameToLayer("UI"), QueryTriggerInteraction.Collide);

                if (hits != null && hits.Length > 0)
                {
                    float nearDistance = float.PositiveInfinity;
                    GameObject nearObj = null;
                    RaycastHit nearHit = hits[0];
                    foreach (RaycastHit hit in hits)
                    {
                        if (hit.distance < nearDistance)
                        {
                            nearDistance = hit.distance;
                            nearObj = hit.transform.gameObject;
                            nearHit = hit;
                        }
                    }

                    float distance = Vector3.Distance(nearHit.point, hand.GetFinger(8).position);
                    if (grabed || (distance <= TouchDistance && TouchDistance > 0f))
                    {
                        uiDetectControl(nearObj, nearHit.point, grabed);
                        return;
                    }
                    else
                    {
                        tempHits.AddRange(hits);
                    }
                }
            }

            if (handIsOpened)
            {
                isGrabDetected_ = false;
            }

            if (tempHits.Count > 0)
            {
                float nearDistance = float.PositiveInfinity;
                GameObject nearObj = null;
                foreach (RaycastHit hit in tempHits)
                {
                    if (hit.distance < nearDistance)
                    {
                        nearDistance = hit.distance;
                        nearObj = hit.transform.gameObject;
                    }
                }

                if (eventSystem.currentSelectedGameObject != nearObj)
                {
                    eventSystem.SetSelectedGameObject(nearObj);
                }
                return;
            }

            if (prevDetectObj_ != null && submitPointerData_ != null)
            {
                GameObject submitObj = ExecuteEvents.GetEventHandler<ISubmitHandler>(prevDetectObj_);
                if (submitObj != null)
                {
                    ExecuteEvents.Execute(submitObj, submitPointerData_, ExecuteEvents.submitHandler);
                    addColliderToSelectable();
                    prevDetectTime_ = Time.time;
                }
                submitPointerData_ = null;
            }

            if (Time.time - prevDetectTime_ > LeaveTime)
            {
                resetSelect();
                prevDetectObj_ = null;

                Selectable[] selectables = Selectable.allSelectablesArray;
                changeEnabledSelectable(true, selectables);
                base.Process();
                reEnabledSelectable(selectables);
            }
        }

        protected override void OnDestroy()
        {
            foreach (Collider collider in colliders_)
            {
                Destroy(collider);
            }

            base.OnDestroy();
        }
    }
}
