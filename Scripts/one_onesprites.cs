using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnboundLib;

public class one_onesprites: MonoBehaviour {

    void Start() {
        UnityEngine.Debug.Log("MapStart!");
        this.ExecuteAfterFrames(20, () => {
            GetComponentsInChildren<SpriteRenderer>().ToList().ForEach(sr => sr.color=new Color(0.6f, 0.6f, 0.6f, 1));
            GetComponentsInChildren<SpriteMask>().ToList().ForEach(Destroy);
            PlayerManager.instance.players.ForEach(player => player.GetComponent<PlayerJump>().upForce*=2);
        });
    }
    void OnDestroy() {
        UnityEngine.Debug.Log("MapEnd!");
        PlayerManager.instance.players.ForEach(player => player.GetComponent<PlayerJump>().upForce/=2);
    }

}
