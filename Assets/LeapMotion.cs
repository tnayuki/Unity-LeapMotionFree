using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using WebSocketSharp;

public class LeapMotion : MonoBehaviour
{
		protected WebSocket ws;
		protected int handsCount = 0;
		protected Vector3[] handPositions = new Vector3 [2];
		protected int pointablesCount = 0;
		protected Vector3[] pointablePositions = new Vector3 [10];
		protected string lastGestureType;
		// Use this for initialization
		void Start ()
		{
				ws = new WebSocket ("ws://localhost:6437/v4.json");
		
				// called when websocket messages come.
				ws.OnMessage += (sender, e) =>
				{
						try {
								
								var jsonData = MiniJSON.Json.Deserialize (e.Data) as Dictionary<string,object>;
								//Dictionary<string, object> jsonData = (Dictionary<string, object>)parser.ParseJSON (e.Data);

								if (jsonData.ContainsKey ("version")) {
										Debug.Log ("Protocol Version: " + jsonData ["version"].ToString ());
								}

								if (jsonData.ContainsKey ("hands")) {
										List<object> hands = (List<object>)jsonData ["hands"];

										handsCount = hands.Count;
										for (int i = 0; i < hands.Count; i++) { 
												Dictionary<string, object> hand = (Dictionary<string, object>)hands [i];
												List<object> position = (List<object>)hand ["palmPosition"];
												handPositions [i] = new Vector3 (-Convert.ToSingle(position [0]), Convert.ToSingle(position [1]), Convert.ToSingle(position [2]));
										}
								}

								if (jsonData.ContainsKey ("gestures")) {
										List<object> gestures = (List<object>)jsonData ["gestures"];
										if (gestures.Count > 0) {
												Dictionary<string, object> gesture = (Dictionary<string, object>)gestures [0];

												if (!gesture ["type"].Equals (lastGestureType)) {
														Debug.Log (gesture ["type"]);
														lastGestureType = (string)gesture ["type"];
												}
										}
								}

								if (jsonData.ContainsKey ("pointables")) {
										List<object> pointables = (List<object>)jsonData ["pointables"];

										pointablesCount = pointables.Count;
										for (int i = 0; i < pointables.Count; i++) { 
												Dictionary<string, object> pointable = (Dictionary<string, object>)pointables [i];
												List<object> position = (List<object>)pointable ["tipPosition"];
												pointablePositions [i] = new Vector3 (-Convert.ToSingle(position [0]), Convert.ToSingle(position [1]), Convert.ToSingle(position [2]));new Vector3 (-Convert.ToSingle(position [0]), Convert.ToSingle(position [1]), Convert.ToSingle(position [2]));
										}
								}
						} catch (Exception ex) {
								Debug.LogException (ex);
						}
				};

				ws.OnOpen += (sender, e) => 
				{
						Debug.Log ("Connected to LeapMotion Server");

						ws.Send ("{\"focused\": true }");
						ws.Send ("{\"enableGestures\": true }");
				};

				ws.ConnectAsync ();
		}
	
		// Update is called once per frame
		void Update ()
		{
		}

		void OnDestroy ()
		{
				ws.CloseAsync ();
		}

		void OnDrawGizmos ()
		{
				for (int i = 0; i < handsCount; i++) { 
						Gizmos.color = Color.red;
						Gizmos.DrawWireSphere (handPositions [i] / 10, 1);
				}

				for (int i = 0; i < pointablesCount; i++) { 
						Gizmos.color = Color.green;
						Gizmos.DrawWireSphere (pointablePositions [i] / 10, 0.5f);
				}
		}
}
