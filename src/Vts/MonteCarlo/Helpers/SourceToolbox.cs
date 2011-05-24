﻿using System;
using System.IO;
using Vts.Common;
using Vts.Extensions;
using Vts.MonteCarlo.Sources;

namespace Vts.MonteCarlo.Helpers
{
    /// <summary>
    /// Utilities shared by Sources.
    /// </summary>
    public class SourceToolbox
    {
        /// <summary>
        /// Update the direction and position after beam rotation, source axis rotation and translation
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="dir"></param>
        /// <param name="beamRotation"></param>
        /// <param name="translate"></param>
        /// <param name="sourceAxisRotation"></param>
        /// <param name="flags"></param>
        public static void UpdateDirectionAndPositionAfterGivenFlags(
            ref Position pos,
            ref Direction dir,
            PolarAzimuthalAngles sourceAxisRotation,
            Position translate,
            PolarAzimuthalAngles beamRotation,
            SourceFlags flags)
        {
            if (flags.RotationOfPrincipalSourceAxisFlag)
                UpdateDirectionAfterRotationByGivenPolarAndAzimuthalAngles(beamRotation, dir); 
            if (flags.beamRotationFromInwardNormalFlag)
                DoSourceRotationByGivenPolarAndAzimuthalAngle(sourceAxisRotation, ref dir, ref pos);            
            if (flags.TranslationFromOriginFlag)
                UpdatePositionAfterTranslation(ref pos, translate);
        }

        /// <summary>
        /// Update the direction and position after beam axis rotation and translation
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="dir"></param>
        /// <param name="translate"></param>
        /// <param name="rotateBeam"></param>
        /// <param name="flags"></param>
        public static void UpdateDirectionAndPositionAfterGivenFlags(
            ref Position pos,
            ref Direction dir,
            PolarAzimuthalAngles sourceAxisRotation,
            Position translate,
            SourceFlags flags)
        {
            if (flags.beamRotationFromInwardNormalFlag)
                DoSourceRotationByGivenPolarAndAzimuthalAngle(sourceAxisRotation, ref dir, ref pos); 
            if (flags.TranslationFromOriginFlag)
                UpdatePositionAfterTranslation(ref pos, translate);
        }

                
        /// <summary>
        /// Returns a random position in a line (Flat distribution)        
        /// </summary>
        /// <param name="center">The center coordiantes of the line</param>
        /// <param name="lengthX">The x-length of the line</param>        
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Position GetRandomFlatLinePosition(
            Position center,
            double lengthX,
            Random rng)
        {
            if (lengthX == 0.0)
            {
                return (center);
            }

            return (new Position(
            center.X + GetRandomFlatLocationOfSymmetricalLine(lengthX, rng),
            center.Y,
            center.Z));
        }

        
        /// <summary>
        /// Returns a random position in a line (Flat distribution)        
        /// </summary>
        /// <param name="center">The center coordiantes of the line</param>
        /// <param name="lengthX">The x-length of the line</param>   
        /// <param name="stdev">The standard deviation of normal distribution</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Position GetRandomGaussianLinePosition(
            Position center,
            double lengthX,
            double beamDiaFWHM,
            Random rng)
        {
            if (lengthX == 0.0)
            {
                return (center);
            }
            else
            {
                double factor = lengthX / beamDiaFWHM;
                return (
                    new Position(
                        center.X + 0.8493218 * beamDiaFWHM * OneGaussianDistributedRandomNumber(
                            GetLowerLimit(factor), 
                            rng),
                        center.Y,
                        center.Z));
            }

        }

        /// <summary>
        /// Returns a random position in a rectangular surface (Flat distribution)
        /// </summary>
        /// <param name="center">The center coordiantes of the rectangle</param>
        /// <param name="lengthX">The x-length of the rectangle</param>
        /// <param name="lengthY">The y-length of the rectangle</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Position GetRandomFlatRectangulePosition(
            Position center,
            double lengthX,
            double lengthY,
            Random rng)
        {
            if ((lengthX == 0.0) && (lengthY == 0.0))
            {
                return (center);
            }

            var position = new Position { Z = center.Z };

            position.X = center.X + GetRandomFlatLocationOfSymmetricalLine(lengthX, rng);
            position.Y = center.Y + GetRandomFlatLocationOfSymmetricalLine(lengthY, rng);
            return position;
        }

        /// <summary>
        /// Returns a random position in a rectangular surface (Gaussian distribution)
        /// </summary>
        /// <param name="center">The center coordiantes of the rectangle</param>
        /// <param name="lengthX">The x-length of the rectangle</param>
        /// <param name="lengthY">The y-length of the rectangle</param>
        /// <param name="stdevX">The standard deviation of normal distribution</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Position GetRandomGaussianRectangulePosition(
            Position center,
            double lengthX,
            double lengthY,
            double beamDiaFWHM,
            Random rng)
        {
            if ((lengthX == 0.0) && (lengthY == 0.0))
            {
                return (center);
            }

            var position = new Position { Z = center.Z };
            double factor = lengthX/beamDiaFWHM;

            position.X = center.X + 0.8493218 * beamDiaFWHM * OneGaussianDistributedRandomNumber(
                GetLowerLimit(factor),
                rng);

            factor = lengthY/beamDiaFWHM;
            position.Y = center.Y + 0.8493218 * beamDiaFWHM * OneGaussianDistributedRandomNumber(
                GetLowerLimit(factor),
                rng);
            return position;
        }

        /// <summary>
        /// Returns a random position in a cuboid volume (Flat distribution)
        /// </summary>
        /// <param name="center">The center coordiantes of the cuboid</param>
        /// <param name="lengthX">The x-length of the cuboid</param>
        /// <param name="lengthY">The y-length of the cuboid</param>
        /// <param name="lengthZ">The z-length of the cuboid</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Position GetRandomFlatCuboidPosition(
            Position center,
            double lengthX,
            double lengthY,
            double lengthZ,
            Random rng)
        {
            if ((lengthX == 0.0) && (lengthY == 0.0) && (lengthZ == 0.0))
            {
                return (center);
            }

            var position = new Position { };

            position.X = center.X + GetRandomFlatLocationOfSymmetricalLine(lengthX, rng);
            position.Y = center.Y + GetRandomFlatLocationOfSymmetricalLine(lengthY, rng);
            position.Z = center.Z + GetRandomFlatLocationOfSymmetricalLine(lengthZ, rng);
            return position;
        }


        /// <summary>
        /// Returns a random position in a cuboid volume (Gaussian distribution)
        /// </summary>
        /// <param name="center">The center coordiantes of the cuboid</param>
        /// <param name="lengthX">The x-length of the cuboid</param>
        /// <param name="stdevX">The standard deviation of the distribution along the x-axis</param>
        /// <param name="lengthY">The y-length of the cuboid</param>
        /// <param name="stdevY">The standard deviation of the distribution along the y-axis</param>
        /// <param name="lengthZ">The z-length of the cuboid</param>
        /// <param name="stdevZ">The standard deviation of the distribution along the z-axis</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Position GetRandomGaussianCuboidPosition(
            Position center,
            double lengthX,
            double lengthY,
            double lengthZ,
            double beamDiaFWHM,
            Random rng)
        {
            if ((lengthX == 0.0) && (lengthY == 0.0) && (lengthZ == 0.0))
            {
                return (center);
            }

            Position position = new Position(0, 0, 0);

            double factor = lengthX / beamDiaFWHM;
            position.X = center.X + 0.8493218 * beamDiaFWHM * OneGaussianDistributedRandomNumber(
                GetLowerLimit(factor),
                rng);

            factor = lengthY / beamDiaFWHM;
            position.Y = center.Y + 0.8493218 * beamDiaFWHM * OneGaussianDistributedRandomNumber(
                GetLowerLimit(factor),
                rng);

            factor = lengthZ / beamDiaFWHM;
            position.Z = center.Z + 0.8493218 * beamDiaFWHM * OneGaussianDistributedRandomNumber(
                GetLowerLimit(factor),
                rng);
            return position;
        }

        /// <summary>
        /// Returns a random position in a ellipsoid volume (Flat distribution)
        /// </summary>
        /// <param name="center">The center coordiantes of the ellipse</param>
        /// <param name="lengthX">The x-length of the ellipse</param>
        /// <param name="lengthY">The y-length of the ellipse</param>
        /// <param name="lengthZ">The z-length of the ellipse</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Position GetRandomFlatEllipsoidPosition(
            Position center,
            double lengthX,
            double lengthY,
            double lengthZ,
            Random rng)
        {
            if ((lengthX == 0.0) && (lengthY == 0.0) && (lengthZ == 0.0))
            {
                return (center);
            }

            var position = new Position { };
            double radius;
            do
            {
                position = GetRandomFlatCuboidPosition(center,
                    lengthX,
                    lengthY,
                    lengthZ,
                    rng);
                radius = (4.0 * position.X * position.X / (lengthX * lengthX) +
                          4.0 * position.Y * position.Y / (lengthY * lengthY) +
                          4.0 * position.Z * position.Z / (lengthZ * lengthZ));
            } while (radius <= 1.0);
            return position;
        }


        /// <summary>
        /// Returns a random position in an ellipsoid volume (Gaussian distribution)
        /// </summary>
        /// <param name="center">The center coordiantes of the cuboid</param>
        /// <param name="lengthX">The x-length of the cuboid</param>        
        /// <param name="lengthY">The y-length of the cuboid</param>
        /// <param name="lengthZ">The z-length of the cuboid</param>
        /// <param name="stdev">The standard deviation of the distribution along the z-axis</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Position GetRandomGaussianEllipsoidPosition(
            Position center,
            double lengthX,
            double lengthY,
            double lengthZ,
            double stdev,
            Random rng)
        {
            if ((lengthX == 0.0) && (lengthY == 0.0) && (lengthZ == 0.0))
            {
                return (center);
            }

            var position = new Position { };
            double radius;
            do
            {
                position = GetRandomGaussianCuboidPosition(center,
                    lengthX,
                    lengthY,
                    lengthZ,
                    stdev,
                    rng);

                radius = (4.0 * position.X * position.X / (lengthX * lengthX) +
                          4.0 * position.Y * position.Y / (lengthY * lengthY) +
                          4.0 * position.Z * position.Z / (lengthZ * lengthZ));
            } while (radius <= 1.0);
            return position;
        }

        /// <summary>
        /// Provides a random position in a annular circle (Flat distribution)
        /// </summary>
        /// <param name="center">The center coordiantes of the circle</param>
        /// <param name="innerRadius">The inner radius of the circle</param>
        /// <param name="outerRadius">The outer radius of the circle</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Position GetRandomFlatCirclePosition(
            Position center,
            double innerRadius,
            double outerRadius,
            Random rng)
        {
            if (outerRadius == 0.0)
            {
                return (center);
            }
            double RN1 = 2 * Math.PI * rng.NextDouble();
            double RN2 = Math.Sqrt(innerRadius * innerRadius + (outerRadius * outerRadius - innerRadius * innerRadius) * rng.NextDouble());
            return (new Position(
                center.X + RN2 * Math.Cos(RN1),
                center.Y + RN2 * Math.Sin(RN1),
                0.0));
        }

        /// <summary>
        /// Provides a random position in a circle (Gaussisan distribution)
        /// <summary>
        /// <param name="center">The center coordiantes of the circle</param>
        /// <param name="outerRadius">The outer radius of the circle</param>
        /// <param name="stdev">The standard deviation of the distribution</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>       
        public static Position GetRandomGaussianCirclePosition(
            Position center,
            double outerRadius,
            double beamDiaFWHM,
            Random rng)
        {
            if (outerRadius == 0.0)
            {
                return (center);
            }

            double x = 0.0;
            double y = 0.0;
            double factor = outerRadius / beamDiaFWHM;

            TwoGaussianDistributedRandomNumbers(
                ref x,
                ref y,
                GetLowerLimit(factor),
                rng);

            return (new Position(
                center.X + 0.8493218 * beamDiaFWHM * x,
                center.Y + 0.8493218 * beamDiaFWHM * y,
                center.Z));
        }

        /// <summary>
        /// Provides a random position in an ellipse (Flat distribution)
        /// </summary>
        /// <param name="center">The center coordiantes of the ellipse</param>
        /// <param name="a">'a' parameter of the ellipse</param>
        /// <param name="b">'b' parameter of the ellipse</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Position GetRandomFlatEllipsePosition(
            Position center,
            double a,
            double b,
            Random rng)
        {
            if ((a == 0.0) && (b == 0.0))
            {
                return (center);
            }

            double x, y;
            /*eliminate points outside the ellipse */
            do
            {
                x = a * (2.0 * rng.NextDouble() - 1);
                y = b * (2.0 * rng.NextDouble() - 1);
            }
            while ((x * x / (a * a)) + (y * y / (b * b)) <= 1.0);
            return (new Position(
                center.X + a * x,
                center.Y + b * y,
                0.0));
        }

        /// <summary>
        /// Provides a random position in an ellipse (Gaussian distribution)
        /// </summary>
        /// <param name="center">The center coordiantes of the ellipse</param>
        /// <param name="a">'a' parameter of the ellipse</param>
        /// <param name="b">'b' parameter of the ellipse</param>
        /// <param name="stdev">The standard deviation of normal distribution</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>   
        public static Position GetRandomGaussianEllipsePosition(
            Position center,
            double a,
            double b,
            double beamDiaFWHM,
            Random rng)
        {
            if ((a == 0.0) && (b == 0.0))
            {
                return (center);
            }

            double x, y;
            double factor1 = 2 * a / beamDiaFWHM;
            double factor2 = 2 * b / beamDiaFWHM;


            /*eliminate points outside the ellipse */
            do
            {
                x = OneGaussianDistributedRandomNumber(
                    GetLowerLimit(factor1),
                    rng);
                y = OneGaussianDistributedRandomNumber(
                    GetLowerLimit(factor2),
                    rng);
            }
            while ((x * x / (a * a)) + (y * y / (b * b)) <= 1.0);

            return (new Position(
                center.X + a * x,
                center.Y + b * y,
                center.Z));
        }


        /// <summary>
        /// Provides a direction for a given two dimensional position and a polar angle
        /// </summary>
        /// <param name="polarAngle">Constant polar angle</param>
        /// <param name="position">The position </param>
        /// <returns></returns>
        public static Direction GetDirectionForGiven2DPositionAndPolarAngle(
            double polarAngle,
            Position position)
        {
            double radius = Math.Sqrt(position.X * position.X + position.Y * position.Y);

            if (radius == 0.0)
                return(new Direction(
                    0.0,
                    0.0,
                    Math.Cos(polarAngle)));
            else
                return (new Direction(
                    position.X / radius,
                    position.Y / radius,
                    Math.Cos(polarAngle)));
        }

        /// <summary>
        /// Provides a direction for a given random polar angle range and random azimuthal angle range
        /// </summary>
        /// <param name="polarAngleEmissionRange">The polar angle range</param>
        /// <param name="azimuthalAngleEmissionRange">The azimuthal angle range</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static Direction GetRandomDirectionForPolarAndAzimuthalAngleRange(
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Random rng)
        {
            double cost, sint, phi, cosp, sinp;

            if ((polarAngleEmissionRange.Start == polarAngleEmissionRange.Stop) && (azimuthalAngleEmissionRange.Start == azimuthalAngleEmissionRange.Stop))
                return (new Direction(0.0, 0.0, 1.0));
            else
            {
                //sampling cost           
                cost = rng.NextDouble(Math.Cos(polarAngleEmissionRange.Start), Math.Cos(polarAngleEmissionRange.Stop));
                sint = Math.Sqrt(1.0 - cost * cost);

                //sampling phi
                phi = rng.NextDouble(azimuthalAngleEmissionRange.Start, azimuthalAngleEmissionRange.Stop);
                cosp = Math.Cos(phi);
                sinp = Math.Sin(phi);

                return (new Direction(
                    sint * cosp,
                    sint * sinp,
                    cost));
            }
        }

        
        /// <summary>
        /// Provides a polarazimuthal angle pair for a given uniform random polar angle range and random azimuthal angle range
        /// </summary>
        /// <param name="polarAngleEmissionRange">The polar angle range</param>
        /// <param name="azimuthalAngleEmissionRange">The azimuthal angle range</param>
        /// <param name="rng">The random number generato</param>
        /// <returns></returns>
        public static PolarAzimuthalAngles GetRandomPolarAzimuthalForUniformPolarAndAzimuthalAngleRange(
            DoubleRange polarAngleEmissionRange,
            DoubleRange azimuthalAngleEmissionRange,
            Random rng)
        {
            return (new PolarAzimuthalAngles(
                Math.Acos(rng.NextDouble(Math.Cos(polarAngleEmissionRange.Start), Math.Cos(polarAngleEmissionRange.Stop))),
                rng.NextDouble(azimuthalAngleEmissionRange.Start, azimuthalAngleEmissionRange.Stop)));
        }

        /// <summary>
        /// Provides a random direction for a isotropic point source
        /// </summary>
        /// <param name="rng">The random number generato</param>
        /// <returns></returns>
        public static Direction GetRandomDirectionForIsotropicDistribution(Random rng)
        {
            double cost, sint, phi, cosp, sinp;

            //sampling cost           
            cost = rng.NextDouble(0, Math.PI);
            sint = Math.Sqrt(1.0 - cost * cost);

            //sampling phi
            phi = rng.NextDouble(0, 2 * Math.PI);
            cosp = Math.Cos(phi);
            sinp = Math.Sin(phi);

            return (new Direction(
                sint * cosp,
                sint * sinp,
                cost));
        }

        /// <summary>
        /// Update the direction and position of the source after rotating around the x-axis
        /// </summary>
        /// <param name="xRotation">rotation angle around the x-axis</param>
        /// <param name="currentDirection">The direction to be updated</param>
        /// <param name="currentPosition">The position to be updated</param>
        public static void DoSourceRotationAroundXAxis(
            double xRotation,
            ref Direction currentDirection,
            ref Position currentPosition)
        {
            // readability eased with local copies of following
            double uy = currentDirection.Uy;
            double uz = currentDirection.Uz;
            double y = currentPosition.Y;
            double z = currentPosition.Z;

            double cost, sint;    /* cosine and sine of rotation angle */

            cost = Math.Cos(xRotation);
            sint = Math.Sqrt(1.0 - cost * cost);

            currentDirection.Uy = uy * cost - uz * sint;
            currentDirection.Uz = uy * sint + uz * cost;

            currentPosition.Y = y * cost - z * sint;
            currentPosition.Z = y * sint + z * cost;
        }

        /// <summary>
        /// Update the direction and position of the source after rotating around the y-axis
        /// </summary>
        /// <param name="yRotation">rotation angle around the y-axis</param>
        /// <param name="currentDirection">The direction to be updated</param>
        /// <param name="currentPosition">The position to be updated</param>
        public static void DoSourceRotationAroundYAxis(
            double yRotation,
            ref Direction currentDirection,
            ref Position currentPosition)
        {
            // readability eased with local copies of following
            double ux = currentDirection.Ux;
            double uz = currentDirection.Uz;
            double x = currentPosition.X;
            double z = currentPosition.Z;

            double cost, sint;    /* cosine and sine of rotation angle */

            cost = Math.Cos(yRotation);
            sint = Math.Sqrt(1.0 - cost * cost);

            currentDirection.Ux = ux * cost + uz * sint;
            currentDirection.Uz = -ux * sint + uz * cost;

            currentPosition.X = x * cost + z * sint;
            currentPosition.Z = -x * sint + z * cost;
        }

        /// <summary>
        /// Update the direction and position of the source after rotating around the z-axis
        /// </summary>
        /// <param name="zRotation">rotation angle around the z-axis</param>
        /// <param name="currentDirection">The direction to be updated</param>
        /// <param name="currentPosition">The position to be updated</param>
        public static void DoSourceRotationAroundZAxis(
            double zRotation,
            ref Direction currentDirection,
            ref Position currentPosition)
        {
            // readability eased with local copies of following
            double ux = currentDirection.Ux;
            double uy = currentDirection.Uy;
            double x = currentPosition.X;
            double y = currentPosition.Y;

            double cost, sint;    /* cosine and sine of rotation angle */

            cost = Math.Cos(zRotation);
            sint = Math.Sqrt(1.0 - cost * cost);

            currentDirection.Ux = ux * cost - uy * sint;
            currentDirection.Uy = ux * sint + uy * cost;

            currentPosition.X = x * cost - y * sint;
            currentPosition.Y = x * sint + y * cost;
        }



        /// <summary>
        /// Update the direction and position of the source after rotating by a given polar and azimuthal angle
        /// </summary>
        /// <param name="theta">polar angle</param>
        /// <param name="phi">azimuthal angle</param>
        /// <param name="currentDirection">The direction to be updated</param>
        /// <param name="currentPosition">The position to be updated</param>
        public static void DoSourceRotationByGivenPolarAndAzimuthalAngle(
            PolarAzimuthalAngles rotationAngle,
            ref Direction currentDirection,
            ref Position currentPosition)
        {
            // readability eased with local copies of following
            double ux = currentDirection.Ux;
            double uy = currentDirection.Uy;
            double uz = currentDirection.Uz;
            double x = currentPosition.X;
            double y = currentPosition.Y;
            double z = currentPosition.Z;

            double cost, sint, cosp, sinp;    /* cosine and sine of theta and phi */

            cost = Math.Cos(rotationAngle.Theta);
            cosp = Math.Cos(rotationAngle.Phi);
            sint = Math.Sqrt(1.0 - cost * cost);
            sinp = Math.Sqrt(1.0 - cosp * cosp);

            currentDirection.Ux = ux * cosp * cost - uy * sint + uz * sinp * cost;
            currentDirection.Uy = ux * cosp * sint + uy * cost + uz * sinp * sint;
            currentDirection.Uz = -ux * sinp + uz * cost;

            currentPosition.X = x * cosp * cost - y * sint + z * sinp * cost;
            currentPosition.Y = x * cosp * sint + y * cost + z * sinp * sint;
            currentPosition.Z = -x * sinp + z * cost;
        }

        /// <summary>
        /// Provide the direction after rotating around the x-axis
        /// </summary>
        /// <param name="xRotation">rotation angle around the x-axis</param>
        /// <param name="currentDirection">The direction to be updated</param>
        /// <param name="currentPosition">The position to be updated</param>
        public static Direction GetDirectionAfterRotationAroundXAxis(
            double xRotation,
            Direction currentDirection)
        {
            // readability eased with local copies of following
            double uy = currentDirection.Uy;
            double uz = currentDirection.Uz;

            double cost, sint;    /* cosine and sine of rotation angle */

            cost = Math.Cos(xRotation);
            sint = Math.Sin(xRotation);

            currentDirection.Uy = uy * cost - uz * sint;
            currentDirection.Uz = uy * sint + uz * cost;
            return currentDirection;
        }

        /// <summary>
        /// Provide the direction after rotating around the y-axis
        /// </summary>
        /// <param name="yRotation">rotation angle around the y-axis</param>
        /// <param name="currentDirection">The direction to be updated</param>
        /// <param name="currentPosition">The position to be updated</param>
        public static Direction GetDirectionAfterRotationAroundYAxis(
            double yRotation,
            Direction currentDirection)
        {
            // readability eased with local copies of following
            double ux = currentDirection.Ux;
            double uz = currentDirection.Uz;

            double cost, sint;    /* cosine and sine of rotation angle */

            cost = Math.Cos(yRotation);
            sint = Math.Sin(yRotation);

            currentDirection.Ux = ux * cost + uz * sint;
            currentDirection.Uz = -ux * sint + uz * cost;
            return currentDirection;
        }

        /// <summary>
        /// Provide the direction after rotating around the z-axis
        /// </summary>
        /// <param name="zRotation">rotation angle around the z-axis</param>
        /// <param name="currentDirection">The direction to be updated</param>
        /// <returns></returns>
        public static Direction GetDirectionAfterRotationAroundZAxis(
            double zRotation,
            Direction currentDirection)
        {
            // readability eased with local copies of following
            double ux = currentDirection.Ux;
            double uy = currentDirection.Uy;

            double cost, sint;    /* cosine and sine of rotation angle */

            cost = Math.Cos(zRotation);
            sint = Math.Sin(zRotation);

            currentDirection.Ux = ux * cost - uy * sint;
            currentDirection.Uy = ux * sint + uy * cost;
            return currentDirection;
        }

        ///// <summary>
        ///// Provide the direction after rotating around three axis
        ///// </summary>
        ///// <param name="xRotation">rotation angle around the x-axis</param>
        ///// <param name="yRotation">rotation angle around the y-axis</param>
        ///// <param name="zRotation">rotation angle around the z-axis</param>
        ///// <param name="currentDirection">The direction to be updated</param>
        ///// <returns></returns>
        //public static void UpdateDirectionAfterRotationAroundThreeAxisClockwiseLeftHanded(
        //    ThreeAxisRotation rotationAngles,
        //    Direction currentDirection)
        //{
        //    // readability eased with local copies of following
        //    double ux = currentDirection.Ux;
        //    double uy = currentDirection.Uy;
        //    double uz = currentDirection.Uz;

        //    double cosx, sinx, cosy, siny, cosz, sinz;    /* cosine and sine of rotation angles */

        //    cosx = Math.Cos(rotationAngles.XRotation);
        //    cosy = Math.Cos(rotationAngles.YRotation);
        //    cosz = Math.Cos(rotationAngles.ZRotation);
        //    sinx = Math.Sqrt(1.0 - cosx * cosx);
        //    siny = Math.Sqrt(1.0 - cosy * cosy);
        //    sinz = Math.Sqrt(1.0 - cosz * cosz);

        //    currentDirection.Ux = ux * cosy * cosz + uy * (-cosx * sinz + sinx * siny * cosz) + uz * (sinx * sinz + cosx * siny * cosz);
        //    currentDirection.Uy = ux * cosy * sinz + uy * (cosx * cosz + sinx * siny * sinz) + uz * (-sinx * cosz + cosx * siny * sinz);
        //    currentDirection.Uz = ux * siny + uy * sinx * cosy + uz * cosx * cosy;
        //}

        /// <summary>
        /// Provide the direction after rotating by given polar and azimuthal angle
        /// </summary>
        /// <param name="theta">polar angle</param>
        /// <param name="phi">azimuthal angle</param>
        /// <param name="currentDirection">The direction to be updated</param>
        /// <returns></returns>
        public static void UpdateDirectionAfterRotationByGivenPolarAndAzimuthalAngles(
            PolarAzimuthalAngles rotationAngle,
            Direction currentDirection)
        {
            // readability eased with local copies of following
            double ux = currentDirection.Ux;
            double uy = currentDirection.Uy;
            double uz = currentDirection.Uz;

            double cost, sint, cosp, sinp;    /* cosine and sine of theta and phi */

            cost = Math.Cos(rotationAngle.Theta);
            cosp = Math.Cos(rotationAngle.Phi);
            sint = Math.Sqrt(1.0 - cost * cost);
            sinp = Math.Sin(rotationAngle.Phi);

            currentDirection.Ux = ux * cosp * cost - uy * sint + uz * sinp * cost;
            currentDirection.Uy = ux * cosp * sint + uy * cost + uz * sinp * sint;
            currentDirection.Uz = -ux * sinp + uz * cost;
        }

        public static PolarAzimuthalAngles GetPolarAndAzimuthalAnglesFromDirection(
            Direction direction)
        {
            if (direction == SourceDefaults.DefaultDirectionOfPrincipalSourceAxis)
            {
                return new PolarAzimuthalAngles(0.0, 0.0);
            }

            double x, y, z, r, theta, phi;
            x = direction.Ux;
            y = direction.Uy;
            z = direction.Uz;

            theta = Math.Acos(z);

            if ((x != 0.0) || (y != 0.0))
            {
                r = Math.Sqrt(x * x + y * y);

                if (y >= 0.0)
                    phi = Math.Acos(x / r);
                else
                    phi = 2 * Math.PI - Math.Acos(x / r);
            }
            else
                phi = 0;        

            PolarAzimuthalAngles polarAzimuthalAngles = new PolarAzimuthalAngles(
                theta,
                phi);

            return polarAzimuthalAngles;
        }
              
        /// <summary>
        /// Provides a flat random location of a symmetrical line
        /// </summary>
        /// <param name="length">The length of the line</param>
        /// <param name="rng">The random number generator</param>
        /// <returns></returns>
        public static double GetRandomFlatLocationOfSymmetricalLine(
            double length,
            Random rng)
        {
            return length * (rng.NextDouble() - 0.5);
        }


        /// <summary>
        /// Generate one normally (Gaussian) distributed random number by using Box Muller Algorithm (with sine/cosine)
        /// </summary>
        /// <param name="nrng1">normally distributed random number</param>
        /// <param name="lowerLimit">lower limit of the uniform random number</param>
        /// <param name="rng">The random number generator</param>
        public static double OneGaussianDistributedRandomNumber(
            double lowerLimit,
            Random rng)
        {
            return Math.Sqrt(-2 * Math.Log(rng.NextDouble(lowerLimit, 1.0))) * Math.Cos(2 * Math.PI * rng.NextDouble());
        }

        /// <summary>
        /// Generate two normally (Gaussian) distributed random numbers by using Box Muller Algorithm (with sine/cosine)
        /// </summary>
        /// <param name="nrng1">normally distributed random number 1</param>
        /// <param name="nrng2">normally distributed random number 2</param>
        /// <param name="lowerLimit">lower limit of the uniform random number</param>
        /// <param name="rng">The random number generator</param>
        public static void TwoGaussianDistributedRandomNumbers(
            ref double nrng1,
            ref double nrng2,
            double lowerLimit,
            Random rng)
        {
            double RN1, RN2;

            RN1 = 2 * Math.PI * rng.NextDouble();
            RN2 = Math.Sqrt(-2 * Math.Log(rng.NextDouble(lowerLimit, 1.0)));

            nrng1 = RN2 * Math.Cos(RN1);
            nrng2 = RN2 * Math.Sin(RN1);
        }


        /// <summary>
        /// Generate three normally (Gaussian) distributed random numbers by using Box Muller Algorithm (with sine/cosine)
        /// </summary>
        /// <param name="nrng1">normally distributed random number 1</param>
        /// <param name="nrng2">normally distributed random number 2</param>
        /// <param name="nrng3">normally distributed random number 3</param>
        /// <param name="lowerLimit">lower limit of the uniform random number</param>
        /// <param name="rng">The random number generator</param>
        //public static void ThreeGaussianDistributedRandomNumbers(
        //    ref double nrng1,
        //    ref double nrng2,
        //    ref double nrng3,
        //    double lowerLimit,
        //    Random rng)
        //{
        //    nrng1 = OneGaussianDistributedRandomNumber(lowerLimit, rng);
        //    TwoGaussianDistributedRandomNumbers(ref nrng2, ref nrng3, lowerLimit, rng);
        //}


        /// <summary>
        /// Returns the new position after translation
        /// </summary>
        /// <param name="oldPosition">The old location</param>
        /// <param name="translation">Translation coordinats relative to the origin</param>
        /// <returns></returns>
        public static void UpdatePositionAfterTranslation(
            ref Position oldPosition,
            Position translation)
        {
            oldPosition.X += translation.X;
            oldPosition.Y += translation.Y;
            oldPosition.Z += translation.Z;
        }


        public static double NAToPolarAngle(double numericalAperture)
        {
            return Math.Asin(numericalAperture);
        }

        
        public static double GetLowerLimit(double factor)
        {
            return (Math.Exp(-0.5*factor*factor));
        }
    }
}
