// Anexe este script a todo sprite 2D inserido no mundo 3D (personagens, itens, obstáculos).
// O objeto com SpriteRenderer deve ser filho de um pivot vazio para funcionar corretamente.

using UnityEngine;

namespace VivaParintins.VFX
{
    public class BillboardController : MonoBehaviour
    {
        public enum BillboardMode { FullFacing, YAxisLocked }

        [Tooltip("YAxisLocked mantém o sprite na vertical — recomendado para personagens.")]
        public BillboardMode mode = BillboardMode.YAxisLocked;

        Camera _cam;

        void Awake()
        {
            _cam = Camera.main;
        }

        void LateUpdate()
        {
            if (_cam == null)
            {
                _cam = Camera.main;
                if (_cam == null) return;
            }

            if (mode == BillboardMode.FullFacing)
            {
                transform.rotation = _cam.transform.rotation;
            }
            else
            {
                // Trava eixo Y: sprite fica ereto mas sempre de frente para a câmera
                Vector3 dir = _cam.transform.position - transform.position;
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.001f)
                    transform.rotation = Quaternion.LookRotation(-dir);
            }
        }
    }
}
