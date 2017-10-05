using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSteeringManager : MonoBehaviour {

    public GameObject player;
    public float maxVelocity = 1.0f;
    public float maxForce = 1.0f;
    public float maxSpeed = 1.0f;
    public float speed = 0.1f;
    public float mass = 1.0f;
    public float slowRadius;
    public float maxSeeAhead;
    public float maxAvoidanceForce;
    public float maxSeperation;


    //Vector3 position;
    Vector3 target;
    Vector3 ahead;
    Vector3 ahead2;
    Vector3 avoidanceForce;
    Vector3 velocity;

    public GameObject threat;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        target = player.transform.TransformPoint(0, 0, -10);
    }

    // Update is called once per frame
    void Update()
    {
        target = player.transform.TransformPoint(0, 0, -10);
        avoidanceForce = collisionAvoidance();
        threat = findMostThreateningObstacle();

        Ahead(target);
        Seek(target, slowRadius);
    }

    void Steering(Vector3 p_Velocity, float p_Speed)
    {
        //speed = Random.Range(0.1f, maxSpeed);

        transform.position += p_Velocity * Time.deltaTime * p_Speed;

        //transform.Translate(0, 0, Time.deltaTime * speed);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(player.transform.position), 3 * Time.deltaTime);
    }

    void Seek(Vector3 target, float radius)
    {
        Vector3 position = transform.position;
        Vector3 velocity = Vector3.Normalize(target - position);
        float distance = Vector3.Distance(target, position);
        Vector3 desiredVelocity = Vector3.zero;
        Vector3 steering = Vector3.zero;

        desiredVelocity = target - position;

        if (distance < radius)
        {
            desiredVelocity = Vector3.Normalize(desiredVelocity) * maxVelocity * (distance / radius);
        }
        else
        {
            desiredVelocity = Vector3.Normalize(desiredVelocity) * maxVelocity;
        }

        steering = desiredVelocity - velocity;
        steering = Vector3.ClampMagnitude(steering, maxForce);
        steering = steering + Seperation() + collisionAvoidance();
        steering = steering / mass;
        velocity = Vector3.ClampMagnitude(velocity + steering, maxSpeed);

        transform.position += velocity * Time.deltaTime * speed;
        //Steering(velocity, speed);
        transform.LookAt(player.transform.position);
    }

    void Flee()
    {
        Vector3 velocity = Vector3.Normalize(transform.position - target);
        Vector3 desiredVelocity = Vector3.Normalize(transform.position - target) * maxVelocity;
        Vector3 steering = desiredVelocity - velocity;

        steering = Vector3.ClampMagnitude(steering, maxForce);
        steering = steering / mass;
        velocity = Vector3.ClampMagnitude(velocity + steering, maxSpeed);

        transform.position += velocity * Time.deltaTime * speed;
        transform.LookAt(-player.transform.position);
    }

    void calcAvoidanceForce(GameObject go)
    {
        avoidanceForce = ahead - go.transform.position;
        avoidanceForce = Vector3.Normalize(avoidanceForce) * maxAvoidanceForce;
    }

    void Ahead(Vector3 target)
    {
        Vector3 position;
        position = transform.position;
        velocity = Vector3.Normalize(target - position);
        ahead = position + Vector3.Normalize(velocity) * maxSeeAhead;
        ahead2 = position + Vector3.Normalize(velocity) * maxSeeAhead * 0.5f;
    }

    Vector3 collisionAvoidance()
    {
        GameObject mostThreatening = findMostThreateningObstacle();
        //Mesh mesh;
        Vector3 avoidance = Vector3.zero;
        //Collider col;

        if (mostThreatening != null)
        {
            //mesh = mostThreatening.GetComponent<MeshFilter>().mesh;
            //Vector3[] vertices = mesh.vertices;
            //Vector2[] uvs = new Vector2[vertices.Length];
            //Bounds bounds = mesh.bounds;

            ////testing out different ways to avoid shit

            //avoidance.x += ahead.x - bounds.size.x;
            //avoidance.y += ahead.y - bounds.size.y;
            //avoidance.z += ahead.z - bounds.size.z;

            //print(bounds.size.x);
            //print(mostThreatening.transform.position.x);

            //col = mostThreatening.GetComponent<Collider>();
            //Bounds bounds = col.bounds;

            //avoidance.x += ahead.x - bounds.size.x;
            //avoidance.y += ahead.y - bounds.size.y;
            //avoidance.z += ahead.z - bounds.size.z;

            //avoidance.x += ahead.x - mostThreatening.transform.position.x;
            //avoidance.y += ahead.y - mostThreatening.transform.position.y;
            //avoidance.z += ahead.z - mostThreatening.transform.position.z;

            avoidance += ahead - mostThreatening.transform.position;

            avoidance.Normalize();
            avoidance.Scale(Vector3.one * maxAvoidanceForce);
        }
        else
        {
            avoidance.Scale(Vector3.zero);
        }

        return avoidance;
    }

    GameObject findMostThreateningObstacle()
    {
        GameObject mostThreatening = null;
        GameObject[] objects = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];

        for (int i = 0; i < objects.Length; i++)
        {
            GameObject obstacle = objects[i];

            bool collision = lineIntersectsCircle(ahead, ahead2, obstacle);
            if (collision && (mostThreatening == null || Vector3.Distance(transform.position, obstacle.transform.position) < Vector3.Distance(transform.position, mostThreatening.transform.position)))
            {
                mostThreatening = obstacle;
            }
        }

        return mostThreatening;
    }

    bool lineIntersectsCircle(Vector3 a, Vector3 b, GameObject go)
    {
        if (go.GetComponent<Collider>() != null)
        {
            Collider col = go.GetComponent<Collider>();
            float radius = col.bounds.extents.magnitude;
            return Vector3.Distance(go.transform.position, a) <= radius || Vector3.Distance(go.transform.position, b) <= radius;
        }
        else
            return false;
        
    }

    Vector3 Seperation()
    {
        Vector3 force = Vector3.zero;
        int neighbourCount = 0;

        GameObject[] objects = GameObject.FindGameObjectsWithTag("Bird");

        for(int i = 0; i < objects.Length; i++)
        {
            if(objects[i]!= this && Vector3.Distance(objects[i].transform.position,this.transform.position) <= maxSeperation)
            {
                //force += objects[i].transform.position - this.transform.position;

                force = force + (this.transform.position - objects[i].transform.position);
                neighbourCount++;
            }
        }

        if(neighbourCount != 0)
        {
            force /= neighbourCount;

            force = Vector3.ClampMagnitude(force, -1);
        }

        force.Normalize();
        force = Vector3.ClampMagnitude(force, maxSeperation);


        return force;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, ahead2);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(ahead2, ahead);
        //Gizmos.color = Color.black;
        //Gizmos.DrawLine(transform.position, target);
    }

    public float getAngle(Vector3 vector)
    {
        return Mathf.Atan2(vector.y, vector.x);
    }

    public void setAngle(Vector3 vector, float value)
    {
        float len = vector.magnitude;
        vector.x = Mathf.Cos(value) * len;
        vector.y = Mathf.Sin(value) * len;
    }

    public void truncate(Vector3 vector, float max)
    {
        float i;

        i = max / vector.magnitude;
        i = i < 1.0f ? i : 1.0f;

        scaleBy(vector, i);
    }

    void scaleBy(Vector3 vector ,float k)
    {
        vector.x *= k;
        vector.y *= k;
    }
}
