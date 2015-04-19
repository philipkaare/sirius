using System;
using Microsoft.SPOT;

namespace Sirius
{
    public class Kalman
    {
        const int x0_0 = 0;
        const int x0_1 = 1;
        const int x1_0 = 2;
        const int x1_1 = 3;

        private double Q_angle = 0.001f; // Process noise variance for the accelerometer
        private double Q_bias = 0.003f; // Process noise variance for the gyro bias
        private double R_measure = 0.03f; // Measurement noise variance - this is actually the variance of the measurement noise
        private double angle = 0.0f; // The angle calculated by the Kalman filter - part of the 2x1 state vector
        private double bias = 0.0f; // The gyro bias calculated by the Kalman filter - part of the 2x1 state vector
        private double rate; // Unbiased rate calculated from the rate and the calculated bias - you have to call getAngle to update the rate
        private double[] P = new double[4]; // Error covariance matrix - This is a 2x2 matrix

        // The angle should be in degrees and the rate should be in degrees per second and the delta time in seconds
        public double getAngle(double newAngle, double newRate, double dt)
        {
            // KasBot V2 - Kalman filter module - http://www.x-firm.com/?page_id=145
            // Modified by Kristian Lauszus
            // See my blog post for more information: http://blog.tkjelectronics.dk/2012/09/a-practical-approach-to-kalman-filter-and-how-to-implement-it
            // Discrete Kalman filter time update equations - Time Update ("Predict")
            // Update xhat - Project the state ahead
            /* Step 1 */
            rate = newRate - bias;
            angle += dt * rate;
            // Update estimation error covariance - Project the error covariance ahead
            /* Step 2 */
            P[x0_0] += dt * (dt * P[x1_1] - P[x0_1] - P[x1_0] + Q_angle);
            P[x0_1] -= dt * P[x1_1];
            P[x1_0] -= dt * P[x1_1];
            P[x1_1] += Q_bias * dt;
            // Discrete Kalman filter measurement update equations - Measurement Update ("Correct")
            // Calculate Kalman gain - Compute the Kalman gain
            /* Step 4 */
            double S = P[x0_0] + R_measure; // Estimate error
            /* Step 5 */
            double[] K = new double[2]; // Kalman gain - This is a 2x1 vector
            K[0] = P[x0_0] / S;
            K[1] = P[x1_0] / S;
            // Calculate angle and bias - Update estimate with measurement zk (newAngle)
            /* Step 3 */
            double y = newAngle - angle; // Angle difference
            /* Step 6 */
            angle += K[0] * y;
            bias += K[1] * y;
            // Calculate estimation error covariance - Update the error covariance
            /* Step 7 */
            double P00_temp = P[x0_0];
            double P01_temp = P[x0_1];
            P[x0_0] -= K[0] * P00_temp;
            P[x0_1] -= K[0] * P01_temp;
            P[x1_0] -= K[1] * P00_temp;
            P[x1_1] -= K[1] * P01_temp;
            return angle;
        }

        void setAngle(double newAngle) { angle = newAngle; }// Used to set angle, this should be set as the starting angle
        double getRate() { return rate; } // Return the unbiased rate
        /* These are used to tune the Kalman filter */
        void setQangle(double newQ_angle) { Q_angle = newQ_angle; }
        void setQbias(double newQ_bias) { Q_bias = newQ_bias; }
        void setRmeasure(double newR_measure) { R_measure = newR_measure; }
        double getQangle() { return Q_angle; }
        double getQbias() { return Q_bias; }
        double getRmeasure() { return R_measure; }
    }
}
