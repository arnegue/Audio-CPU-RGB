using System;

namespace AudioCPURGB {
    /**
     * Small class which contains a name for an Audio-RGB-algorithm and it's method
     */
    class AudioAlgorithm {
        private String _name;
        public Action _method { get; }

        public AudioAlgorithm(String name, Action method) {
            _name = name;
            _method = method;
        }

        public override String ToString() {
            return _name;
        }
    }
}
