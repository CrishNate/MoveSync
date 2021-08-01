using UnityEngine;

namespace MoveSync
{
    public class BulletShapeWallProjectile : BulletShapeProjectile
    {
        private static int SizeX = 4;
        private static int SizeY = 6;
        
        public override void Init(ProjectileParam initParam)
        {
            customShape = true;
            
            base.Init(initParam);

            float c = 1f / Mathf.Max(SizeX, SizeY) * radius;
            
            for (int x = 0; x < SizeX; x ++)
            for (int y = 0; y < SizeY; y ++)
            {
                GameObject bulletModel = Instantiate(bulletInstanceModel, transform);
                bulletModel.transform.localPosition = new Vector3(x, y - (SizeY - 1) * 0.5f, 0) * c;
                bulletModel.GetComponent<MeshFilter>().sharedMesh = initParam.shape;

                directions.Add(bulletModel, bulletModel.transform.localPosition);
            }
        }
    }
}