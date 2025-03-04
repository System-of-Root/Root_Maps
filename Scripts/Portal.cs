using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnboundLib;
using UnityEngine;

namespace Root_Maps.Scripts {
    internal class Portal: MonoBehaviour {
        public bool Exit = false;
        public int PortalID = 0;
        public static Dictionary<int,Portal> Exits= new Dictionary<int,Portal>();

        public void Awake() {
            if(Exit) {
                Exits[PortalID] = this;
            }
        }
        public void Update() {
            if(Exit)
                return;
            Collider2D[] array = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x*3);

            for(int i = 0; i<array.Length; i++) {
                if(array[i].GetComponent<NetworkPhysicsObject>() is NetworkPhysicsObject physicsObject && physicsObject.photonView.IsMine) {
                    physicsObject.GetComponent<Rigidbody2D>().velocity = GetRotation()*physicsObject.GetComponent<Rigidbody2D>().velocity;
                    physicsObject.transform.Rotate(GetRotation().eulerAngles);
                    physicsObject.transform.position=Exits[PortalID].transform.position;
                    return;
                }
                if(array[i].GetComponent<PlayerVelocity>() is PlayerVelocity playerVelocity) {
                    playerVelocity.transform.position=Exits[PortalID].transform.position;
                    playerVelocity.SetFieldValue("velocity", (Vector2)(GetRotation()*(Vector2)playerVelocity.GetFieldValue("velocity")));
                    StartCoroutine(PortalFoce(playerVelocity, (Vector2)playerVelocity.GetFieldValue("velocity")));
                    playerVelocity.GetComponentInParent<PlayerCollision>().IgnoreWallForFrames(2);
                    playerVelocity.GetComponent<CharacterData>().sinceGrounded=0.025f;
                    return;
                }
                if(array[i].GetComponentInParent<MoveTransform>() is MoveTransform moveTransform &&array[i].GetComponentInParent<RayCastTrail>() is RayCastTrail rayCast) {
                    moveTransform.velocity = GetRotation() * moveTransform.velocity;
                    moveTransform.transform.position =Exits[PortalID].transform.position;
                    rayCast.SetFieldValue("lastPos", Exits[PortalID].transform.position);
                }
            }
        }
        IEnumerator PortalFoce(PlayerVelocity playerVelocity, Vector2 force) {
            yield return null;
            var c = 0f;
            while(c<0.3f) {
                playerVelocity.SetFieldValue("velocity", (force*TimeHandler.deltaTime*10) +(Vector2)playerVelocity.GetFieldValue("velocity"));
                c+=TimeHandler.deltaTime;
                yield return null;
            }
        }
        public Quaternion GetRotation() {
            return Quaternion.Euler(0, 0, 
                Exits[PortalID].transform.eulerAngles.z - transform.eulerAngles.z);
        }
    }
}
