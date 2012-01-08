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
            if (isLeaf())
                Volume = Body.GetBoundingSphere();
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

        public void FindPotentialCollisions(List<ContactData> Potentials)
        {
            if (this.isLeaf())
                return;
            this.Children[0].FindPotentialCollisionsWith(Potentials, this.Children[1]);
        }

        public void FindPotentialCollisionsWith(List<ContactData> Potentials, BVHNode other)
        {
            if (!CollidesWith(other))
                return;
            if (this.isLeaf() && other.isLeaf())
                Potentials.Add(new ContactData(this.Body, other.Body));
            else if (other.isLeaf() || ((!this.isLeaf()) && (this.Volume.Radius > other.Volume.Radius)))
            {
                this.Children[0].FindPotentialCollisionsWith(Potentials, other);
                this.Children[1].FindPotentialCollisionsWith(Potentials, other);
            }
            else
            {
                other.Children[0].FindPotentialCollisionsWith(Potentials, this.Children[0]);
                other.Children[1].FindPotentialCollisionsWith(Potentials, this.Children[1]);
            }
        }
    }
}