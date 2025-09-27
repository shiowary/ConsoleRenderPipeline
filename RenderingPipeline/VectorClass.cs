using System.ComponentModel;
using System.Numerics;


/*
Lyras Attempt at Vector3 class, based on unitys Vector3 class 
this handles all of the necassary transform vecotore and other math
*/
namespace RenderPipeline
{
     

    public struct Vector3
    {
        public float x; 
        public float y;
        public float z;
        public void set(float x,float y,float z)
        {
            this.x = x; this.y = y; this.z = z;
        }
        
        public Vector3(float x = 0f , float y = 0f, float z = 0f)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        //Returns a normalized vector based on the current vector. The normalized vector has a magnitude of 1 and is in the same direction as the current vector. Returns a zero vector If the current vector is too small to be normalized.

        public Vector3 normalize() {
            float nx = this.x; 
            float ny= this.y;
            float nz = this.z;
            float mag = this.magnitude();
            nx = nx / mag;
            ny = ny / mag;
            nz = nz / mag;
            return new Vector3(nx, ny, nz);
        }

        //magnitude	Returns the length of this vector (Read Only).

        public float magnitude() { 
            return (float)Math.Sqrt((double)Math.Pow((double)this.x, 2) + (double)Math.Pow((double)this.y, 2) + (double)Math.Pow((double)this.z, 2));
        }
        //Returns the squared length of this vector (Read Only).
        public float sqrMagnitude() {
            return (float)Math.Pow((double)this.x, 2) + (float)Math.Pow((double)this.y, 2) + (float)Math.Pow((double)this.z, 2);
        }

        public (int,int,int) GetIntTupel()
        {
            return ((int)this.x, (int)this.y, (int)this.z);
        }
        /*
        operator -	Subtracts one vector from another.
        operator !=	Returns true if vectors are different.
        operator *	Multiplies a vector by a number.
        operator /	Divides a vector by a number.
        operator +	Adds two three-dimensional vectors with component-wise addition.
        operator ==	Returns true if two vectors are approximately equal.
        */

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }
        public static Vector3 operator -(Vector3 a, Vector3 b) {
            return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vector3 operator *(Vector3 a, float b) {
            return new Vector3(a.x*b,a.y*b,a.z*b);
        }

        public static bool operator ==(Vector3 a, Vector3 b) {
            return (a.x == b.x && a.y == b.y && a.z == b.z);
        }

        public static bool operator !=(Vector3 a, Vector3 b) {
            return (a.x != b.x && a.y != b.y && a.z != b.z);
        }

        public static Vector3 operator /(Vector3 a, float b) {
            if (b == 0)
            {
                return a; // error msg would be useful i presume
            }
            return new Vector3(a.x / b, a.y / b, a.z / b);
        }

        //----------- Static Methods (Documentation overview from Unity) -------------------


        //Angle	Calculates the angle between two vectors.

        public static float Angle(Vector3 from, Vector3 to)
        {
            float denominator = (float)Math.Sqrt(from.sqrMagnitude() * to.sqrMagnitude());
            if (denominator < 1e-15f)
                return 0f;
            float dot = Dot(from, to) / denominator;
            dot = Math.Clamp(dot, -1f, 1f);
            return (float)(Math.Acos(dot) * (180.0 / Math.PI));
        }

        //ClampMagnitude	Returns a copy of vector with its magnitude clamped to maxLength.
        public static Vector3 ClampMagnitude(Vector3 vector, float maxLength)
        {
            float sqrMag = vector.sqrMagnitude();
            if (sqrMag > maxLength * maxLength)
            {
                float mag = (float)Math.Sqrt(sqrMag);
                return vector * (maxLength / mag);
            }
            return vector;
        }

        //Cross	Calculates the cross product of two three-dimensional vectors.

        public static Vector3 Cross(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(
                lhs.y * rhs.z - lhs.z * rhs.y,
                lhs.z * rhs.x - lhs.x * rhs.z,
                lhs.x * rhs.y - lhs.y * rhs.x
            );
        }
        //Distance	Calculates the distance between two three-dimensional points.

        public static float Distance(Vector3 a, Vector3 b)
        {
            return (a - b).magnitude();
        }
        //Dot	Calculates the dot product of two three-dimensional vectors defined in the same coordinate space.

        public static float Dot(Vector3 lhs, Vector3 rhs) //left hand side - right hand side
        {
            return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
        }


        //Lerp	Linearly interpolates between two points.

        //LerpUnclamped	Linearly interpolates between two vectors.

        //Max	Returns a vector that is made from the largest components of two vectors.

        //Min	Returns a vector that is made from the smallest components of two vectors.

        //MoveTowards	Calculate a position between the points specified by current and target, moving no farther than the distance specified by maxDistanceDelta.

        //Normalize	Makes this vector have a magnitude of 1.

        //OrthoNormalize	Makes vectors normalized and orthogonal to each other.

        //Project	Projects a vector onto another vector.

        //ProjectOnPlane	Projects a vector onto a plane.
        public static Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeNormal)
        {
            float sqrMag = Dot(planeNormal, planeNormal);
            if (sqrMag < 1e-15f) //magic very small number
                return vector;
            // Projection of vector onto the normal
            Vector3 projection = planeNormal * (Dot(vector, planeNormal) / sqrMag);
            // Subtract it -> projection on plane
            return vector - projection;
        }

        //Reflect	Reflects a vector off the plane defined by a normal.

        //RotateTowards	Rotates a vector current towards target.

        //Scale	Multiplies two vectors component-wise.

        //SignedAngle	Calculates the signed angle between vectors from and to in relation to axis.

        //Slerp	Spherically interpolates between two three-dimensional vectors.

        //SlerpUnclamped	Spherically interpolates between two vectors.

        //SmoothDamp	Gradually changes a vector towards a desired goal over time.




    }


}
