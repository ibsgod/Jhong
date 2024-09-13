using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    public GameObject player;
    public Merchant canBuy = null;
    public GameObject current;

    private void Update() {
        Vector3 lineEnd = transform.position + transform.forward * 4;
        float min = Int32.MaxValue;
        GameObject closest = null;
        foreach(GameObject entity in player.GetComponent<PlayerBehaviour>().worldScript.entities) {
            if (entity == player) {
                continue;
            }
            Vector3 projection = ProjectPointOnLineSegment(transform.position, lineEnd, entity.transform.position);
            if (Vector3.Distance(entity.transform.position, projection) < min) {
                min = Vector3.Distance(entity.transform.position, projection);
                closest = entity;
            }
        }
        if (min > 1) {
            player.GetComponent<PlayerBehaviour>().clearDialogue();
            current = null;
            canBuy = null;
            return;
        }
        if (current == closest) {
            return;
        }
        current = closest;
        if (!closest.name.Contains("merchant")) {
            canBuy = null;
            player.GetComponent<PlayerBehaviour>().makeDialogue("yo its a " + closest.name);
        }
        else {
            if (closest.GetComponent<Merchant>().attacked) {
                player.GetComponent<PlayerBehaviour>().makeDialogue("ok bruh square up then");
                canBuy = null;
            }
            else {
                player.GetComponent<PlayerBehaviour>().makeDialogue("yo u want " + closest.GetComponent<Merchant>().item + " for " + closest.GetComponent<Merchant>().price + "?");
                canBuy = closest.GetComponent<Merchant>();
            }
        }
    }
    public static int PointOnWhichSideOfLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point){
        
        Vector3 lineVec = linePoint2 - linePoint1;
        Vector3 pointVec = point - linePoint1;
        
        float dot = Vector3.Dot(pointVec, lineVec);
        
        //point is on side of linePoint2, compared to linePoint1
        if(dot > 0){
            
            //point is on the line segment
            if(pointVec.magnitude <= lineVec.magnitude){
                
                return 0;
            }
            
            //point is not on the line segment and it is on the side of linePoint2
            else{
                
                return 2;
            }
        }
        
        //Point is not on side of linePoint2, compared to linePoint1.
        //Point is not on the line segment and it is on the side of linePoint1.
        else{
            
            return 1;
        }
    }
    public static Vector3 ProjectPointOnLine(Vector3 linePoint, Vector3 lineVec, Vector3 point){        
        
        //get vector from point on line to point in space
        Vector3 linePointToPoint = point - linePoint;
        
        float t = Vector3.Dot(linePointToPoint, lineVec);
        
        return linePoint + lineVec * t;
    }
    
    //This function returns a point which is a projection from a point to a line segment.
    //If the projected point lies outside of the line segment, the projected point will 
    //be clamped to the appropriate line edge.
    //If the line is infinite instead of a segment, use ProjectPointOnLine() instead.
    public static Vector3 ProjectPointOnLineSegment(Vector3 linePoint1, Vector3 linePoint2, Vector3 point){
        
        Vector3 vector = linePoint2 - linePoint1;
        
        Vector3 projectedPoint = ProjectPointOnLine(linePoint1, vector.normalized, point);
        
        int side = PointOnWhichSideOfLineSegment(linePoint1, linePoint2, projectedPoint);
        
        //The projected point is on the line segment
        if(side == 0){
            
            return projectedPoint;
        }
        
        if(side == 1){
            
            return linePoint1;
        }
        
        if(side == 2){
            
            return linePoint2;
        }
        
        //output is invalid
        return Vector3.zero;
    }   
}
