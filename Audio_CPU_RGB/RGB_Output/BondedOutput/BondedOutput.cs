using AudioCPURGB.RGB_Creator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AudioCPURGB.RGB_Output.BondedOutPut
{
    class BondedOutput : RGB_Output_Interface
    {
        private RGBOutputManager _manager;
        private bool _enabled;
        private List<RGB_Output_Interface> _current_outputs;

        public BondedOutput(RGBOutputManager manager)
        {
            this._manager = manager;
        }
        
        int RGB_Output_Interface.GetAmountRGBs()
        {
            return 1; // thats to much to show individually
        }

        string RGB_Output_Interface.GetName()
        {
            return "BondedOutput"; // TODO add names of currently_used_outputs to it, but the string could get too long
        }

        void RGB_Output_Interface.Initialize()
        {
            _current_outputs = ShowDialog();
            List<RGB_Output_Interface> initialized_outputs = new List<RGB_Output_Interface>(); 
            foreach (var output in _current_outputs)
            {
                try
                {
                    output.Initialize();
                    initialized_outputs.Add(output);
                } catch {
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
            public RGB_Output_Interface Output;
            public RGBOutputCheckbox(RGB_Output_Interface output)
            {
                this.Output = output;
                this.Text = output.GetName();
            }
        }
        private List<RGB_Output_Interface> ShowDialog()
        {
            Form prompt = new Form();
            prompt.AutoSize = true;
            prompt.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            /*
            prompt.Width = 180;
            prompt.Height = 100;*/
            prompt.Text = "Choose outputs";


            FlowLayoutPanel panel = new FlowLayoutPanel();
            panel.AutoSize = true;
            panel.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            foreach (var out_put in _manager.GetAvailableOutputs())
            {
                if (out_put != this) // Don't add itself
                {
                    RGBOutputCheckbox chk = new RGBOutputCheckbox(out_put);
                    panel.Controls.Add(chk);
                }
            }
          
            Button ok = new Button() { Text = "Okay" };
            ok.Click += (sender, e) => { prompt.Close(); };
            //panel.SetFlowBreak(chk, true);
            panel.Controls.Add(ok);
            prompt.Controls.Add(panel);
            prompt.ShowDialog();

            // At this point the user closed the dialog

            var checked_boxes = panel.Controls.OfType<RGBOutputCheckbox>().Where(c => c.Checked);
            List<RGB_Output_Interface> newOutputs = new List<RGB_Output_Interface>();
            foreach (var cbx in checked_boxes)
            {
                newOutputs.Add(cbx.Output);
            }
            return newOutputs;            
        }
    
        bool RGB_Output_Interface.IsEnabled()
        {
            return _enabled;
        }

        void RGB_Output_Interface.SetEnable(bool enable)
        {
            this._enabled = enable;
            foreach (var output in _current_outputs)
            {
                output.SetEnable(enable);
            }
        }

        void RGB_Output_Interface.ShowRGB(RGB_Value rgb)
        {
            foreach (var output in _current_outputs)
            {
                output.ShowRGB(rgb);
            }
        }

        void RGB_Output_Interface.ShowRGBs(RGB_Value[] rgbs)
        {
            foreach (var output in _current_outputs)
            {
                output.ShowRGBs(rgbs);
            }
        }

        void RGB_Output_Interface.Shutdown()
        {
            foreach (var output in _current_outputs)
            {
                output.Shutdown();
            }
        }
    }
}
