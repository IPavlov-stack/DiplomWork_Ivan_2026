using System;

namespace DiplomWork_Ivan_2026.Controllers
{
    public class PidTemperatureController
    {
        public double Kp { get; set; } = 8.5;
        public double Ki { get; set; } = 1.2;
        public double Kd { get; set; } = 0.7;

        public double MinOutput { get; set; } = 0.0;
        public double MaxOutput { get; set; } = 100.0;

        private double _integral;
        private double _previousError;
        private bool _hasPreviousError;

        public double Update(double setpoint, double currentValue, double deltaTime)
        {
            if (deltaTime <= 0)
                deltaTime = 1.0;

            double error = setpoint - currentValue;

            double derivative = 0.0;

            if (_hasPreviousError)
            {
                derivative = (error - _previousError) / deltaTime;
            }

            double candidateIntegral = _integral + error * deltaTime;

            double output = Kp * error + Ki * candidateIntegral + Kd * derivative;

            double clampedOutput = Math.Clamp(output, MinOutput, MaxOutput);

            // Anti-windup
            // Интегралната съставка се обновява само ако изходът не е в насищане, или ако грешката помага да се излезе от насищането
            bool outputIsNotSaturated = Math.Abs(output - clampedOutput) < 0.0001;
            bool outputIsAtMaximumAndErrorIsNegative = clampedOutput >= MaxOutput && error < 0;
            bool outputIsAtMinimumAndErrorIsPositive = clampedOutput <= MinOutput && error > 0;

            if (outputIsNotSaturated ||
                outputIsAtMaximumAndErrorIsNegative ||
                outputIsAtMinimumAndErrorIsPositive)
            {
                _integral = candidateIntegral;
            }

            output =  Kp * error + Ki * _integral + Kd * derivative;

            _previousError = error;
            _hasPreviousError = true;

            return Math.Clamp(output, MinOutput, MaxOutput);
        }

        public void Reset()
        {
            _integral = 0.0;
            _previousError = 0.0;
            _hasPreviousError = false;
        }
    }
}