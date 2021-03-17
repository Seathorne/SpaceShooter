namespace MovementCore.Dynamic
{
    /// <summary>
    /// The desired precision with which to perform physics update calculations.
    /// </summary>
    public enum Precision
    {
        /// <summary>
        /// Use the Newton-Euler 1 approximation to save computational time.
        /// </summary>
        NewtonEuler1,

        /// <summary>
        /// Use the precise "highschool physics" calculation to improve accuracy.
        /// </summary>
        Precise,
    }
}
