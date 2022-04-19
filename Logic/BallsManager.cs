﻿using System.Collections;
using Data;
using LogicLayer.Exceptions;
using InvalidDataException = LogicLayer.Exceptions.InvalidDataException;

namespace LogicLayer
{
    public class BallsManager
    {
        private readonly int _mapWidth;
        private readonly int _mapHeight;
        private readonly ObjectStorage<Ball> _objectStorage = new();
        private readonly int _ballMinRadius;
        private readonly int _ballMaxRadius;

        public BallsManager(int mapWidth, int mapHeight)
        {
            _mapHeight = mapHeight;
            _mapWidth = mapWidth;
            _ballMinRadius = Math.Min(mapHeight, mapWidth) / 60;
            _ballMaxRadius = Math.Max(mapWidth, mapHeight) / 30;

        }

        public int GetMapWidth()
        {
            return _mapWidth;
        }

        public int GetMapHeight()
        {
            return _mapHeight;
        }

        public int GetBallsMinRadius()
        {
            return _ballMinRadius;
        }

        public int GetBallsMaxRadius()
        {
            return _ballMaxRadius;
        }

        public void CreateBall(int ID, int x, int y, int xDirection, int yDirection)
        {
            if (CheckForExistingID(ID)
               || (x < _ballMinRadius || x > _mapWidth - _ballMinRadius
                         || y < _ballMinRadius || y > _mapHeight - _ballMinRadius
                         || yDirection > _mapHeight - _ballMinRadius || yDirection < ((-1) * _mapHeight + _ballMinRadius)
                         || xDirection > _mapWidth - _ballMinRadius || xDirection < ((-1) * _mapWidth + _ballMinRadius)))
            {
                throw new InvalidDataException("The ball parameters entered are invalid");
            }
            else
            {
                Random rnd = new Random();
                Ball newBall = new Ball(ID, x, y, rnd.Next(_ballMinRadius, _ballMaxRadius), xDirection, yDirection);
                _objectStorage.AddBall(newBall);
            }
        }

        public void GenerateRandomBall()
        {
            Random rnd = new Random();
            int xrand = 0, yrand = 0;
            while (xrand == 0 && yrand == 0)
            {
                xrand = rnd.Next(-5, 5);
                yrand = rnd.Next(-5, 5);
            }


            CreateBall(AutoID()
                , rnd.Next(_ballMaxRadius, _mapWidth - _ballMaxRadius)
                , rnd.Next(_ballMaxRadius, _mapHeight - _ballMaxRadius)
                , xrand
                , yrand);
        }

        public void SummonBalls(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                GenerateRandomBall();
            }
        }

        public int AutoID()
        {
            int max = 0;
            foreach (Ball ball in GetAllBalls())
            {
                if (max < ball.GetID())
                {
                    max = ball.GetID();
                }
            }

            return max + 1;
        }

        public void DoTick()
        {
            //TODO: add ball radius to condition
            foreach (Ball ball in GetAllBalls())
            {
                if (ball.XPos + ball.XDirection + ball.Radius < ball.Radius * 2 || ball.XPos + ball.XDirection + ball.Radius > _mapWidth)
                {
                    ball.XDirection = ball.XDirection * (-1);
                }
                if (ball.YPos + ball.YDirection + ball.Radius < ball.Radius * 2 || ball.YPos + ball.YDirection + ball.Radius > _mapHeight)
                {
                    ball.YDirection = ball.YDirection * (-1);
                }
                ball.XPos += ball.XDirection;
                ball.YPos += ball.YDirection;
            }
        }

        public bool CheckForExistingID(int ID)
        {
            foreach (Ball obj in _objectStorage.GetAllBalls())
            {
                if (ID == obj.GetID())
                {
                    return true;
                }
            }

            return false;
        }

        public Ball GetBallByID(int ID)
        {
            foreach (Ball obj in _objectStorage.GetAllBalls())
            {
                if (ID == obj.GetID())
                {
                    return _objectStorage.GetAllBalls().ElementAt(ID);
                }
            }

            throw new InvalidDataException("The ball with the given ID is not in the list");
        }

        public void RemoveBallByID(int ID)
        {
            foreach (Ball obj in _objectStorage.GetAllBalls())
            {
                if (ID == obj.GetID())
                {
                    _objectStorage.RemoveBall(obj);
                    return;
                }
            }

            throw new InvalidDataException("The ball with the given ID is not in the list");
        }

        public List<Ball> GetAllBalls()
        {
            return _objectStorage.GetAllBalls();
        }

        public void ClearMap()
        {
            _objectStorage.ClearStorage();
        }
    }
}