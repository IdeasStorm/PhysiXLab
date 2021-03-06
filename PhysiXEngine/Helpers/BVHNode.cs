﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysiXEngine
{
    public class BVHNode
    {
        /// <summary>
        /// We will describe every thing in this class later.
        /// </summary>

        public BVHNode[] Children = new BVHNode[2];
        public BoundingSphere Volume;
        public Collidable Body;
        public BVHNode Parent;
        public bool DidInternalDetections = false;

        public BVHNode(BVHNode Parent, Collidable Body)
        {
            Children[0] = null;
            Children[1] = null;
            this.Body = Body;
            this.Volume = Body.GetBoundingSphere();
            this.Parent = Parent;
        }

        public bool isLeaf()
        {
            return (Body != null);
        }

        public void RecalculateBoundingVolume()
        {
            if (this.isLeaf())
                Volume = Body.GetBoundingSphere();
            else
                Volume = BoundingSphere.CreateMerged(Children[0].Volume, Children[1].Volume);
            if (Parent != null)
                Parent.RecalculateBoundingVolume();
        }

        public void Insert(Collidable Body)
        {
            if (this.isLeaf())
            {
                Children[0] = new BVHNode(this, this.Body);
                Children[1] = new BVHNode(this, Body);
                this.Body = null;
                RecalculateBoundingVolume();
            }
            else
            {
                BoundingSphere Temp = Body.GetBoundingSphere();
                if (BoundingSphere.CreateMerged(Children[0].Volume, Temp).Radius
                    < BoundingSphere.CreateMerged(Children[1].Volume, Temp).Radius)
                    Children[0].Insert(Body);
                else
                    Children[1].Insert(Body);
            }
        }

        protected bool CollidesWith(BVHNode other)
        {
            return Volume.Intersects(other.Volume);
        }

        public void FindPotentialCollisions(List<Contact> Potentials)
        {
            if (DidInternalDetections)
                return;
            DidInternalDetections = true;
            if (this.isLeaf())
                return;
            this.Children[0].FindPotentialCollisionsWith(Potentials, this.Children[1]);
        }

        public void FindPotentialCollisionsWithPlane(List<Contact> Potentials, HalfSpace P)
        {
            if (this.Volume.Intersects(P.plane) == PlaneIntersectionType.Intersecting)
            {
                if (this.isLeaf())
                {
                    Potentials.Add(new Contact(this.Body, P.plane));
                }
                else
                {
                    this.Children[0].FindPotentialCollisionsWithPlane(Potentials, P);
                    this.Children[1].FindPotentialCollisionsWithPlane(Potentials, P);
                }
            }
        }

        public void FindPotentialCollisionsWith(List<Contact> Potentials, BVHNode other)
        {
            if (!CollidesWith(other))
            {
                if (!this.isLeaf())
                    this.FindPotentialCollisions(Potentials);
                if (!other.isLeaf())
                    other.FindPotentialCollisions(Potentials);
                return;
            }
            if (this.isLeaf() && other.isLeaf())
            {
                Contact temp = new Contact(this.Body, other.Body);
                if (!temp.BothFixed())
                    Potentials.Add(temp);
            }
            else if (other.isLeaf() && !this.isLeaf())
            {
                this.Children[0].FindPotentialCollisionsWith(Potentials, other);
                this.Children[1].FindPotentialCollisionsWith(Potentials, other);
                this.FindPotentialCollisions(Potentials);
            }
            else if (this.isLeaf() && !other.isLeaf())
            {
                other.Children[0].FindPotentialCollisionsWith(Potentials, this);
                other.Children[1].FindPotentialCollisionsWith(Potentials, this);
            }
            else
            {
                other.Children[0].FindPotentialCollisionsWith(Potentials, this.Children[0]);
                other.Children[0].FindPotentialCollisionsWith(Potentials, this.Children[1]);
                other.Children[1].FindPotentialCollisionsWith(Potentials, this.Children[0]);
                other.Children[1].FindPotentialCollisionsWith(Potentials, this.Children[1]);
                this.FindPotentialCollisions(Potentials);
                other.FindPotentialCollisions(Potentials);
            }
        }
    }
}
