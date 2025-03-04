using Photon.Pun;
using UnboundLib;
using UnityEngine;

namespace Root_Maps.Scripts
{
    public class Moving_Object_2 : MonoBehaviour
    {
        private int targetID;

        public Vector2[] positions;

        public float drag = 1f;

        public float spring = 1f;

        public float cap = 1f;

        public float vCap = 1f;

        public float threshold = 1f;

        public float timeAtPos;

        private float counter;

        private Vector2 startPos;

        private Vector2 velocity;

        private bool Started = false;

        private Rigidbody2D rig;

        private Map map;

        private string myKey;

        public bool teleport = false;

        // Start is called before the first frame update
        void Start()
        {
            base.gameObject.layer = 17;
            rig = GetComponent<Rigidbody2D>();
            map = GetComponentInParent<Map>();
            int levelID = (int)map.GetFieldValue("levelID");
            myKey = "MapObect " + levelID + " " + base.transform.GetSiblingIndex() + transform.parent.name;
            MapManager.instance.GetComponent<ChildRPC>().childRPCsInt.Add(myKey, RPCA_SetTargetID);
        }

        private void OnDestroy()
        {
            if ((bool)MapManager.instance)
            {
                MapManager.instance.GetComponent<ChildRPC>().childRPCsInt.Remove(myKey);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (MapTransition.isTransitioning || !map.hasEntered)
            {
                return;
            }
            if(!Started) {
                startPos=base.transform.localPosition;
                Started=true;
                vCap /= map.transform.localScale.x;
                cap /= map.transform.localScale.x;
                return;
            }
            Vector2 vector = positions[targetID] + startPos;
            if(teleport&&targetID==0) {
                base.transform.localPosition =positions[targetID];
                PlayerManager.instance.players.ForEach(player => {
                    var pfg = player.GetComponent<PlayerFollowGround>();
                    if ((Rigidbody2D)pfg.GetFieldValue("lastRig") == rig) {
                        pfg.SetFieldValue("lastRig", null);
                    }
                });
                if(PhotonNetwork.IsMasterClient) {
                    counter+=TimeHandler.deltaTime;
                    if(counter>timeAtPos) {
                        targetID++;
                        if(targetID>=positions.Length) {
                            targetID=0;
                        }
                        MapManager.instance.GetComponent<ChildRPC>().CallFunction(myKey, targetID);
                        counter=0f;
                    }
                }
                return;
            }
            Vector2 direction = (vector - (Vector2)base.transform.localPosition);
            direction.Normalize();
            Vector2 vector2 = vector - (Vector2)base.transform.localPosition;
            vector2 = Vector3.ClampMagnitude(vector2, cap);
            velocity += vector2 * spring * CappedDeltaTime.time;
            velocity -= velocity * drag * CappedDeltaTime.time;
            velocity = (Vector2)Vector3.ClampMagnitude(velocity, vCap);
            base.transform.localPosition += (Vector3)velocity * TimeHandler.deltaTime;
            Vector2 direction2 = (vector - (Vector2)base.transform.localPosition);
            direction2.Normalize();
            if (direction2 - direction != Vector2.zero)
            {
                base.transform.localPosition = vector;
                velocity = Vector2.zero;
            }
            if (!PhotonNetwork.IsMasterClient || !(Vector2.Distance(base.transform.localPosition, vector) < threshold))
            {
                return;
            }
            counter += TimeHandler.deltaTime;
            if (counter > timeAtPos)
            {
                targetID++;
                if (targetID >= positions.Length)
                {
                    targetID = 0;
                }
                MapManager.instance.GetComponent<ChildRPC>().CallFunction(myKey, targetID);
                counter = 0f;
            }
        }

        private void RPCA_SetTargetID(int setValue)
        {
            targetID = setValue;
        }
    }
}
