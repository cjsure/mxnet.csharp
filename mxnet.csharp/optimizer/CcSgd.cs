﻿using System;
using System.Collections.Generic;
using System.Globalization;
using mxnet.csharp.lr_scheduler;

namespace mxnet.csharp.optimizer
{
    public class CcSgd : Optimizer
    {
        private float _momentum;
        private readonly IntPtr _handle;


        public CcSgd(
            float momentum = 0.0f,
            float rescaleGrad = 1,
            Dictionary<int, string> paramIdx2Name = null,
            float wd = 0,
            float clipGradient = -1,
            float learningRate = 0.01F,
            LrScheduler lrScheduler = null,
            Symbol sym = null,
            int beginNumUpdate = 0)
            : base(rescaleGrad,
                paramIdx2Name,
                wd,
                clipGradient,
                learningRate,
                lrScheduler,
                sym,
                beginNumUpdate)
        {
            this._momentum = momentum;

            this._handle = Optimizer._init_cc_optimizer(
                "ccsgd",
                new[]
                {
                    "momentum",
                    "rescale_grad",
                    "clip_gradient"
                },
                new[]
                {
                    momentum.ToString(CultureInfo.InvariantCulture),
                    rescaleGrad.ToString(CultureInfo.InvariantCulture),
                    clipGradient.ToString(CultureInfo.InvariantCulture)
                });
        }

        public override NdArray create_state(int index, NdArray weight)
        {
            return null;
        }

        public override void update(int index, NdArray weight, NdArray grad, NdArray state)
        {
            var lr = this._get_lr(index);
            var wd = this._get_wd(index);
            this._update_count(index);
            Util.CallCheck(NativeMethods.MXOptimizerUpdate(this._handle,
                index,
                weight.Handle,
                grad.Handle,
                lr,
                wd));
        }
    }
}