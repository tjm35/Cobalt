using BulletFury.Data;
using UnityEngine;

namespace BulletFury.Demo
{
    public class TestWeapon : MonoBehaviour
    {
        [SerializeField] private BulletManager bulletManager = null;
        [SerializeField] private float rotateSpeed = 0f;
        
        private void Update()
        {
            if (bulletManager == null)
                return;
            bulletManager.Spawn(transform.position, bulletManager.Plane == BulletPlane.XY ? transform.up : transform.forward);
            
            transform.Rotate(bulletManager.Plane == BulletPlane.XY ? Vector3.forward : Vector3.up, (rotateSpeed * Time.smoothDeltaTime));
        }
    }
}