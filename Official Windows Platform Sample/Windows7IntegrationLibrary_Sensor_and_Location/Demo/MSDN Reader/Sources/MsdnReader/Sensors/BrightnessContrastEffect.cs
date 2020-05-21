// Copyright (c) Microsoft Corporation.  All rights reserved.

using System;
using System.Windows.Media.Effects;
using System.Windows;
using System.Windows.Media;

namespace MsdnReader
{
    public class GrayscaleEffect : ShaderEffect
    {
        private static PixelShader _pixelShader = new PixelShader()
        {
            UriSource = new Uri(@"pack://application:,,,/MsdnReader;component/Resources/GrayscaleEffect.ps")
        };
        
        public GrayscaleEffect()
        {
            PixelShader = _pixelShader;
            UpdateShaderValue(InputProperty); UpdateShaderValue(DesaturationFactorProperty);
        }
        
        public static readonly DependencyProperty InputProperty = 
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(GrayscaleEffect), 0);
        
        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }
        
        public static readonly DependencyProperty DesaturationFactorProperty = 
            DependencyProperty.Register("DesaturationFactor", typeof(double), typeof(GrayscaleEffect), 
                new UIPropertyMetadata(0.0, PixelShaderConstantCallback(0), CoerceDesaturationFactor));
        
        public double DesaturationFactor
        {
            get { return (double)GetValue(DesaturationFactorProperty); }
            set { SetValue(DesaturationFactorProperty, value); }
        }
        
        private static object CoerceDesaturationFactor(DependencyObject d, object value)
        {
            GrayscaleEffect effect = (GrayscaleEffect)d;
            double newFactor = (double)value;
            
            if (newFactor < 0.0 || newFactor > 1.0)
            {
                return effect.DesaturationFactor;
            }
            return newFactor;
        }
    }

    public class BrightnessContrastEffect : ShaderEffect
    {
        public BrightnessContrastEffect()
        {
            PixelShader = m_shader;
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(BrightnessProperty);
            UpdateShaderValue(ContrastProperty);

        }

        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        public static readonly DependencyProperty InputProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(BrightnessContrastEffect), 0);

        public float Brightness
        {
            get { return (float)GetValue(BrightnessProperty); }
            set { SetValue(BrightnessProperty, value); }
        }

        public static readonly DependencyProperty BrightnessProperty =
            DependencyProperty.Register("Brightness", typeof(double), typeof(BrightnessContrastEffect), 
                new UIPropertyMetadata(0.0, PixelShaderConstantCallback(0)));

        public float Contrast
        {
            get { return (float)GetValue(ContrastProperty); }
            set { SetValue(ContrastProperty, value); }
        }

        public static readonly DependencyProperty ContrastProperty =
            DependencyProperty.Register("Contrast", typeof(double), typeof(BrightnessContrastEffect), 
                new UIPropertyMetadata(0.0, PixelShaderConstantCallback(1)));

        private static PixelShader m_shader = new PixelShader()
        {
            UriSource = new Uri(@"pack://application:,,,/MsdnReader;component/resources/brightnesscontrast.ps")
        };

    }
}