// <copyright file="SourceSimulator.cs" company="Sunup">
// Copyright (c) Sunup. All rights reserved.
// </copyright>

namespace Sunup.DataSource.Simulator
{
    using System;
    using System.IO;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using System.Timers;
    using Sunup.DataSource;

    /// <summary>
    /// SourceSimulator.
    /// </summary>
    public class SourceSimulator : DataSource
    {
        private Timer executionTimer;
        private string[] items;
        private SimulatorOptions options;
        private int[] currentStepInt;
        private int[] currentStep;
        private System.Security.Cryptography.RNGCryptoServiceProvider provider = new System.Security.Cryptography.RNGCryptoServiceProvider();

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceSimulator"/> class.
        /// </summary>
        /// <param name="name">Attribute name.</param>
        /// <param name="options">options.</param>
        /// <param name="items">items.</param>
        public SourceSimulator(string name, SimulatorOptions options, string[] items)
            : base(name)
        {
            this.items = items;
            this.options = options;
            this.currentStepInt = new int[options.DataInstanceNumber];
            this.currentStep = new int[options.DataInstanceNumber];

            var intMin = this.options.MinInteger;
            var intMax = this.options.MaxInteger;
            if (intMax > intMin)
            {
                for (var i = 0; i < options.DataInstanceNumber; i++)
                {
                    this.currentStepInt[i] = this.GetRandomInt(intMin, intMax);
                    this.currentStep[i] = options.IntegerStep;
                }
            }
        }

        /// <summary>
        /// Run data source.
        /// </summary>
        public override void Start()
        {
            this.StartExecutionTimer();
        }

        /// <summary>
        /// Stop.
        /// </summary>
        public override void Stop()
        {
            this.StopExecutionTimer();
        }

        /// <summary>
        /// 生成随机字符串.
        /// </summary>
        /// <param name="length">目标字符串的长度.</param>
        /// <param name="useNum">是否包含数字，1=包含，默认为包含.</param>
        /// <param name="useLow">是否包含小写字母，1=包含，默认为包含.</param>
        /// <param name="useUpp">是否包含大写字母，1=包含，默认为包含.</param>
        /// <param name="useSpe">是否包含特殊字符，1=包含，默认为不包含.</param>
        /// <param name="custom">要包含的自定义字符，直接输入要包含的字符列表.</param>
        /// <returns>指定长度的随机字符串.</returns>
        private string GetRandomString(int length, bool useNum, bool useLow, bool useUpp, bool useSpe, string custom)
        {
            byte[] b = new byte[4];
            this.provider.GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            var s = new StringBuilder();
            string str = custom;
            if (useNum == true)
            {
                str += "0123456789";
            }

            if (useLow == true)
            {
                str += "abcdefghijklmnopqrstuvwxyz";
            }

            if (useUpp == true)
            {
                str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            }

            if (useSpe == true)
            {
                str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
            }

            for (int i = 0; i < length; i++)
            {
                s.Append(str.Substring(r.Next(0, str.Length - 1), 1));
            }

            return s.ToString();
        }

        /// <summary>
        /// 生成随机integer.
        /// </summary>
        /// <param name="min">min.</param>
        /// <param name="max">max.</param>
        /// <returns>integer.</returns>
        private int GetRandomInt(int min, int max)
        {
            byte[] b = new byte[4];
            this.provider.GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            return r.Next(min, max);
        }

        /// <summary>
        /// 生成随机bool.
        /// </summary>
        /// <returns>integer.</returns>
        private bool GetRandomBool()
        {
            byte[] b = new byte[4];
            this.provider.GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            return r.Next(2) == 1;
        }

        private void Execute()
        {
            var json = this.ToJsonString();
            if (!string.IsNullOrEmpty(json))
            {
                this.DataSourceReference.DataSet = json;
                this.NotifyAll();
            }
        }

        private void StartExecutionTimer()
        {
            if (this.executionTimer == null)
            {
                var interval = this.options.TimerInterval;
                this.executionTimer = new Timer(interval);
                this.executionTimer.Elapsed += (sender, e) =>
                {
                    this.executionTimer.Enabled = false;
                    try
                    {
                        this.Execute();
                    }
                    catch
                    {
                        // todo : log info;
                    }

                    this.executionTimer.Enabled = true;
                };

                this.executionTimer.Start();
            }
        }

        private void StopExecutionTimer()
        {
            if (this.executionTimer != null)
            {
                this.executionTimer.Stop();
                this.executionTimer.Dispose();
                this.executionTimer = null;
            }
        }

        private string ToJsonString()
        {
            var number = this.options.DataInstanceNumber;
            if (number <= 0)
            {
                return null;
            }

            var json = string.Empty;
            var options = new JsonWriterOptions
            {
                Indented = true,
            };
            using (var stream = new MemoryStream())
            {
                using (var writer = new Utf8JsonWriter(stream, options))
                {
                    writer.WriteStartObject();
                    if (this.options.GenerateIntData)
                    {
                        var intMin = this.options.MinInteger;
                        var intMax = this.options.MaxInteger;
                        var intStep = this.options.IntegerStep;
                        if (intMax > intMin)
                        {
                            for (int i = 1; i <= number; i++)
                            {
                                writer.WriteNumber($"int{i}", this.GetRandomInt(intMin, intMax));
                            }

                            if (intStep > 0)
                            {
                                for (int i = 1; i <= number; i++)
                                {
                                    this.currentStepInt[i - 1] += this.currentStep[i - 1];
                                    if (this.currentStepInt[i - 1] <= intMin)
                                    {
                                        this.currentStepInt[i - 1] = intMin;
                                        this.currentStep[i - 1] = intStep;
                                    }

                                    if (this.currentStepInt[i - 1] >= intMax)
                                    {
                                        this.currentStepInt[i - 1] = intMax;
                                        this.currentStep[i - 1] = -1 * intStep;
                                    }

                                    writer.WriteNumber($"stepint{i}", this.currentStepInt[i - 1]);
                                }
                            }
                        }
                    }

                    if (this.options.GenerateStringData)
                    {
                        for (int i = 1; i <= number; i++)
                        {
                            writer.WriteString($"string{i}", this.GetRandomString(20, true, true, true, true, "simulated_"));
                        }
                    }

                    if (this.options.GenerateBoolData)
                    {
                        for (int i = 1; i <= number; i++)
                        {
                            writer.WriteBoolean($"bool{i}", this.GetRandomBool());
                        }
                    }

                    writer.WriteEndObject();
                }

                json = Encoding.UTF8.GetString(stream.ToArray());
            }

            Diagnostics.Logger.LogTrace("Simulator generates json >>>> " + json);
            return json;
        }
    }
}
