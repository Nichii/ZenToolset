using UnityEngine;

namespace ZenToolset
{
    /// <summary>
    /// Provides additional useful and commonly used math functions
    /// </summary>
    public static class ZenMath
    {
        /// <summary>
        /// This will map a square coordinate to a circle coordinate
        /// </summary>
        /// <param name="squareMap">Square coordinate, where each axis must be between -1f and 1f</param>
        /// <returns>A circle coordinate</returns>
        public static Vector2 SquareToCircleMapping(Vector2 squareMap)
            => SquareToCircleMapping(squareMap.x, squareMap.y);

        /// <summary>
        /// This will map a square coordinate to a circle coordinate
        /// </summary>
        /// <param name="x">X axis, must be between -1f and 1f</param>
        /// <param name="y">Y axis, must be between -1f and 1f</param>
        /// <returns>A circle coordinate</returns>
        public static Vector2 SquareToCircleMapping(float x, float y)
            => new Vector2
            (
                x * Mathf.Sqrt(1f - y * y / 2f),
                y * Mathf.Sqrt(1f - x * x / 2f)
            );

        /// <summary>
        /// This will map a cube coordinate to a sphere coordinate
        /// </summary>
        /// <param name="cubePos">Cube coordinate, where each axis must be between -1f and 1f</param>
        /// <returns>A sphere coordinate</returns>
        public static Vector3 CubeToSphereMapping(Vector3 cubePos)
            => CubeToSphereMapping(cubePos.x, cubePos.y, cubePos.z);

        /// <summary>
        /// This will map a cube coordinate to a sphere coordinate
        /// </summary>
        /// <param name="x">X axis, must be between -1f and 1f</param>
        /// <param name="y">Y axis, must be between -1f and 1f</param>
        /// <param name="z">Z axis, must be between -1f and 1f</param>
        /// <returns>A sphere coordinate</returns>
        public static Vector3 CubeToSphereMapping(float x, float y, float z)
            => new Vector3
            (
                x * Mathf.Sqrt(1f - y * y / 2f - z * z / 2f + y * y * z * z / 3f),
                y * Mathf.Sqrt(1f - x * x / 2f - z * z / 2f + x * x * z * z / 3f),
                z * Mathf.Sqrt(1f - x * x / 2f - y * y / 2f + x * x * y * y / 3f)
            );
    }
}
