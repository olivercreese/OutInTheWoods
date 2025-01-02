using UnityEngine;

namespace Ursaanimation.CubicFarmAnimals
{
    public class AnimationController : MonoBehaviour
    {
        public Animator animator;
        

        void Start()
        {
            animator = GetComponent<Animator>();
        }

        void Update()
        {
            
        }
    }
}
