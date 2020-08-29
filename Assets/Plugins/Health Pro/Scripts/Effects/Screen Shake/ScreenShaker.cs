using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lowscope.HealthPro
{
    [AddComponentMenu("")]
    public class ScreenShaker : MonoBehaviour
    {
        private Quaternion baseRotation;
        private Vector3 basePosition;
        private bool isShaking;

        public virtual void Shake(Vector3 shakePos, Vector3 shakeRot, float duration)
        {
            StopAllCoroutines();

            if (isShaking)
            {
                this.transform.rotation = baseRotation;
                this.transform.position = basePosition;
            }

            StartCoroutine(ShakeCoroutine(shakePos, shakeRot, duration));
        }

        IEnumerator ShakeCoroutine(Vector3 shakePos, Vector3 shakeRot, float duration)
        {
            float t = 0;

            baseRotation = this.transform.rotation;
            basePosition = this.transform.position;

            Vector3 lastPositionOffset = new Vector3();
            Vector3 lastRotationOffset = new Vector3();

            isShaking = true;

            while (t < duration)
            {
                // We check if anything has changed in terms of position or rotation and adjust accordingly.
                // To ensure that the shake isn't taking over.

                yield return new WaitForEndOfFrame();

                // Lets check if any changes have been made at the begin of the frame, if there are
                // Then we readjust the base position to match the new position
                if (this.transform.position != basePosition + lastPositionOffset)
                {
                    basePosition -= (basePosition + lastPositionOffset) - this.transform.position;
                }

                if (this.transform.rotation.eulerAngles != baseRotation.eulerAngles + lastRotationOffset)
                {
                    baseRotation = Quaternion.Euler(baseRotation.eulerAngles - ((baseRotation.eulerAngles + lastRotationOffset) - this.transform.rotation.eulerAngles));
                }

                float durationLeft = t / duration;

                Vector3 rotationOffset = new Vector3
                    (
                    Random.Range(-shakeRot.x, shakeRot.x),
                    Random.Range(-shakeRot.y, shakeRot.y),
                    Random.Range(-shakeRot.z, shakeRot.z)
                    );

                Vector3 positionOffset = new Vector3
                    (
                    Random.Range(-shakePos.x, shakePos.x),
                    Random.Range(-shakePos.y, shakePos.y),
                    Random.Range(-shakePos.z, shakePos.z)
                    );

                lastPositionOffset = positionOffset * (1 - durationLeft);
                lastRotationOffset = rotationOffset * (1 - durationLeft);

                this.transform.rotation = Quaternion.Euler(baseRotation.eulerAngles + lastRotationOffset);
                this.transform.position = basePosition + lastPositionOffset;

                t += Time.deltaTime;
            }

            this.transform.rotation = Quaternion.Euler(baseRotation.eulerAngles);
            this.transform.position = basePosition;

            isShaking = false;
        }
    }
}