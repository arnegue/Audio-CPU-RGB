using AudioCPURGB.RGBCreator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AudioCPURGB.RGBOutput.BondedOutPut
{
    class BondedOutput : IRGBOutput
    {
        private RGBOutputManager _manager;
        private bool _enabled;
        private List<IRGBOutput> _current_outputs;

        public BondedOutput(RGBOutputManager manager)
        {
            this._manager = manager;
        }

        public int GetAmountRGBs()
        {
            return 1; // thats too much to show individually
        }

        public string GetName()
        {
            return "BondedOutput";
        }

        public void Initialize()
        {
            _current_outputs = ShowDialog();
            List<IRGBOutput> initialized_outputs = new List<IRGBOutput>(); 
            foreach (var output in _current_outputs)
            {
                try
                {
                    output.Initialize();
                    initialized_outputs.Add(output);
                } catch (RGBOutputException) {
                    // Safety shutdown for every already initialized output
                    foreach (var initialized_output in initialized_outputs)
                    {
                        initialized_output.Shutdown();
                    }
                }
            }
        }

        private class RGBOutputCheckbox : CheckBox
        {
            public IRGBOutput Output;
            public RGBOutputCheckbox(IRGBOutput output)
            {
                this.Output = output;
                this.Text = output.GetName();
            }
        }
        private List<IRGBOutput> ShowDialog()
        {
            Form prompt = new Form
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Text = String.Format(System.Globalization.CultureInfo.CurrentCulture, "Choose outputs")
            };

            Panel panel = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            foreach (var out_put in _manager.GetAvailableOutputs())
            {
                if (out_put != this) // Don't add itself
                {
                    RGBOutputCheckbox chk = new RGBOutputCheckbox(out_put);
                    panel.Controls.Add(chk);
                }
            }
          
            Button ok = new Button() { Text = String.Format(System.Globalization.CultureInfo.CurrentCulture, "Okay") };
            ok.Click += (sender, e) => { prompt.Close(); };
            panel.Controls.Add(ok);
            prompt.Controls.Add(panel);
            prompt.ShowDialog();

            // At this point the user closed the dialog
            var checked_boxes = panel.Controls.OfType<RGBOutputCheckbox>().Where(c => c.Checked);
            prompt.Dispose();
            if (!checked_boxes.Any())
            {
                throw new Exception("Can't set BondedOutput: No outputs chosen.");
            }
            List<IRGBOutput> newOutputs = new List<IRGBOutput>();
            foreach (var cbx in checked_boxes)
            {
                newOutputs.Add(cbx.Output);
            }
            return newOutputs;            
        }
    
        public bool IsEnabled()
        {
            return _enabled;
        }

        public void SetEnable(bool enable)
        {
            this._enabled = enable;
            foreach (var output in _current_outputs)
            {
                output.SetEnable(enable);
            }
        }

        public void ShowRGB(RGBValue rgb)
        {
            foreach (var output in _current_outputs)
            {
                output.ShowRGB(rgb);
            }
        }

        public void ShowRGBs(RGBValue[] rgbs)
        {
            this.ShowRGB(rgbs[0]); // Since it's only one rgb, just make it as old protocol 
        }

        public void Shutdown()
        {
            foreach (var output in _current_outputs)
            {
                output.Shutdown();
            }
        }
    }
}
