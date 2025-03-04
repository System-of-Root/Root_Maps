using UnityEngine;

namespace Root_Maps.Scripts {
    public class Kill_On : MonoBehaviour
    {
        public BoxCollider2D box;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < PlayerManager.instance.players.Count; i++)
            {
                if (box.OverlapPoint(PlayerManager.instance.players[i].transform.position) && PlayerManager.instance.players[i].data.view.IsMine)
                {
                    float Damage = PlayerManager.instance.players[i].data.health + 5;
                    PlayerManager.instance.players[i].data.healthHandler.TakeDamage(Vector2.up * Damage, base.transform.position);
                }
            }
        }
    }
}
