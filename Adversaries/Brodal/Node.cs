namespace AdversaryExperiments.Adversaries.Brodal
{
    class Node
    {
        public enum VisitState
        {
            Unvisited,
            Complete,
            VisitingLeft,
            VisitingRight
        }

        private VisitState _state;
        private long _currentEpoch;
        private bool _isSentinel;
            
        public Node Left { get; private set; }
        public Node Right { get; private set; }

        public Node(bool isSentinel)
        {
            _state = VisitState.Unvisited;
            _currentEpoch = long.MaxValue;
            _isSentinel = isSentinel;
            if (!_isSentinel)
            {
                Left = CreateSentinel();
                Right = CreateSentinel();
            }
        }
            
        public VisitState GetState(long epoch)
        {
            if (_isSentinel)
            {
                return VisitState.Complete;
            }
            return epoch == _currentEpoch ? _state : VisitState.Unvisited;
        }

        public void SetState(VisitState state, long epoch)
        {
            _state = state;
            _currentEpoch = epoch;
        }
            
        private static Node CreateSentinel()
        {
            var result = new Node(true);
            return result;
        }

        public void EnsureInitialized()
        {
            if (_isSentinel)
            {
                _isSentinel = false;
                Left = CreateSentinel();
                Right = CreateSentinel();
            }
        }
    }
}