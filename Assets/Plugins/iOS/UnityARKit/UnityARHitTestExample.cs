using System;
using System.Collections.Generic;

namespace UnityEngine.XR.iOS
{
    public class UnityARHitTestExample : MonoBehaviour
    {
        public Transform m_HitTransform;
        public float walkSpeed;
        public float rotateSpeed;
        public String animWalkKeyword;

        private Vector3 targetPosition;
        private Quaternion targetRotation;
        private Animator anim;

        bool HitTestWithResultType(ARPoint point, ARHitTestResultType resultTypes)
        {
            List<ARHitTestResult> hitResults = UnityARSessionNativeInterface.GetARSessionNativeInterface().HitTest(point, resultTypes);
            if (hitResults.Count > 0)
            {
                foreach (var hitResult in hitResults)
                {
                    Debug.Log("Got hit!");
                    targetPosition = UnityARMatrixOps.GetPosition(hitResult.worldTransform);
                    targetRotation = UnityARMatrixOps.GetRotation(hitResult.worldTransform);
                    return true;
                }
            }
            return false;
        }

        void Start()
        {
            anim = GetComponent<Animator>();
        }

        void Update()
        {
            if (Input.touchCount > 0 && m_HitTransform != null)
            {
                var touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    var screenPosition = Camera.main.ScreenToViewportPoint(touch.position);
                    ARPoint point = new ARPoint
                    {
                        x = screenPosition.x,
                        y = screenPosition.y
                    };

                    // prioritize reults types
                    ARHitTestResultType[] resultTypes = {
                        ARHitTestResultType.ARHitTestResultTypeExistingPlaneUsingExtent
                        // if you want to use infinite planes use this:
                        //ARHitTestResultType.ARHitTestResultTypeExistingPlane,
                        //ARHitTestResultType.ARHitTestResultTypeHorizontalPlane, 
                        //ARHitTestResultType.ARHitTestResultTypeFeaturePoint 
                        //ARHitTestResultType.ARHitTestResultTypeVerticalPlane
                    };

                    foreach (ARHitTestResultType resultType in resultTypes)
                    {
                        if (HitTestWithResultType(point, resultType))
                        {
                            return;
                        }
                    }
                }
            }

			Move();

#if UNITY_EDITOR
			if (m_HitTransform.position == new Vector3(0, 0, 0))
			{
				targetPosition = new Vector3(2, 0, 1);
			}
			if (m_HitTransform.position == new Vector3(2, 0, 1))
			{
				targetPosition = new Vector3(-1, 0, 3);
			}
			if (m_HitTransform.position == new Vector3(-1, 0, 3))
			{
				targetPosition = new Vector3(2, 0, 2);
			}
#endif
		}

        void Move () {
            if (m_HitTransform.position != targetPosition) {
				RotateTowardsTarget();
                anim.SetFloat(animWalkKeyword, walkSpeed / 10);
                m_HitTransform.position = Vector3.MoveTowards(m_HitTransform.position, targetPosition, walkSpeed * Time.deltaTime);
                //m_HitTransform.position = targetPosition;
            } else {
                anim.SetFloat(animWalkKeyword, 0);
            }
        }

        void RotateTowardsTarget () {
            Vector3 lookDirection = targetPosition - m_HitTransform.position;
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
            m_HitTransform.rotation = Quaternion.RotateTowards(m_HitTransform.rotation, lookRotation, rotateSpeed * Time.deltaTime);
        }
	
	}
}

