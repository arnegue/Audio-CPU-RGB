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

    class AudioAlgorithm2
    {
        // TODO maybe not the values, but the gui-elements?
        public RGB_Value my_callback(byte[] specArray, int min_slider, int max_slider, int min_trigger, bool absNotRel)
        {
            return new RGB_Value(0, 0, 0);
        }
    }
}
