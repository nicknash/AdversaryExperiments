﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace AdversaryExperiments.Adversaries.Zamir
{
    public class ZamirTernaryAdversary : IAdversary
    {
        private const int Equal = 0;
        private const int Less = -1;
        private const int Greater = 1;
   
        public string Name { get; }
        public long NumComparisons { get; private set; }
        public List<WrappedInt> CurrentData { get; }

        private Node _root;

        public ZamirTernaryAdversary(int size)
        {
            Name = "Zamir3";
            CurrentData = new List<WrappedInt>(Enumerable.Range(0, size).Select(i => new WrappedInt { Value = i }));
            _root = Node.CreateUnit();
        }
        
        public int Compare(WrappedInt x, WrappedInt y)
        {
            ++NumComparisons;
            if(x == y)
            {
                return Equal;
            }

            var xIsAncestor = ExistsPath(x, y);
            var yIsAncestor = ExistsPath(y, x);

            var inSameNode = xIsAncestor && yIsAncestor;
            var requiresClassification = xIsAncestor || yIsAncestor;

            var isXPaired = TryGetPair(x, out var xPair);
            var isYPaired = TryGetPair(y, out var yPair);


            if (requiresClassification)
            {
                if (inSameNode)
                {
                    // Handle the pairing cases (can I devise a little algorithm that makes them
                    // a special case of the distinct node pairs cases?)

                }
                else
                {
                    if (!isXPaired && !isYPaired)
                    {
                        // These are the 'minimal push down' case of the ancestor
                        var result = xIsAncestor ? PushDown(x, y) : OtherSense(PushDown(y, x));
                        return result;
                    }
                    else
                    {
                        // Somewhat complicated cases I need to go through here.
                    }
                }
            }
            //else
            {
                // TODO: The ordering is already defined, return it.

                return -1;
            }
        }

        private int PushDown(WrappedInt ancestor, WrappedInt descendant)
        {
            if(CanPushLeft(ancestor, descendant))
            {
                return Less;
            }
            else if(CanPushRight(ancestor, descendant))
            {
                return Greater;
            }
            else
            {
                if(CanPushTwiceLeft(ancestor, descendant))
                {
                    return Less;
                }
                if(CanPushTwiceRight(ancestor, descendant))
                {
                    return Greater;
                }
                throw new Exception($"Should never happen: Cannot resolve comparison of two unpaired elements in distinct nodes.");
            }
        }

        private int OtherSense(int r)
         => r switch 
         {
             Greater => Less,
             Less => Greater,
             _ => throw new Exception($"Unexpected comparison result {r}")
         };

        private Node GetNode(WrappedInt v) => throw new Exception("TODO1");

        private bool TryGetPair(WrappedInt v, out WrappedInt w) => throw new Exception("TODO2");

        private bool ExistsPath(WrappedInt x, WrappedInt y) => throw new Exception("TODO3");

        private bool ExistsPath(Node x, Node y) => throw new Exception($"TODO4");
 
        private bool CanPushLeft(Node x, Node y) => !ExistsPath(x.Left, y) && !ExistsPath(y, x.Left);
        private bool CanPushRight(Node x, Node y) => throw new Exception($"TODO");

        private bool CanPushLeft(WrappedInt x, WrappedInt y) => CanPushLeft(GetNode(x), GetNode(y));

        private bool CanPushTwiceLeft(WrappedInt x, WrappedInt y) => CanPushLeft(GetNode(x).Left, GetNode(y));

        private bool CanPushRight(WrappedInt x, WrappedInt y) => throw new Exception($"TODO");

        private bool CanPushTwiceRight(WrappedInt x, WrappedInt y) => throw new Exception($"TODO");


    }
}