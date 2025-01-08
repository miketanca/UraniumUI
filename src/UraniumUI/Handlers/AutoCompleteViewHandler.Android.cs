﻿#if ANDROID
using Android.Content;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System.Collections;
using UraniumUI.Controls;

namespace UraniumUI.Handlers;

public partial class AutoCompleteViewHandler : ViewHandler<IAutoCompleteView, AppCompatAutoCompleteTextView>
{
    protected override AppCompatAutoCompleteTextView CreatePlatformView()
    {
        var autoComplete = new AppCompatAutoCompleteTextView(Context)
        {
            Text = VirtualView?.Text,
        };

        GradientDrawable gd = new GradientDrawable();
        gd.SetColor(global::Android.Graphics.Color.Transparent);
        autoComplete.SetBackground(gd);
        autoComplete.SetSingleLine(true);
        autoComplete.ImeOptions = ImeAction.Done;
        autoComplete.Threshold = VirtualView.Threshold;

        if (VirtualView != null)
        {
            autoComplete.SetTextColor(VirtualView.TextColor.ToPlatform());
        }

        return autoComplete;
    }

    protected override void ConnectHandler(AppCompatAutoCompleteTextView platformView)
    {
        platformView.TextChanged += PlatformView_TextChanged;
        platformView.FocusChange += PlatformView_FocusChange;
        platformView.ItemClick += PlatformView_ItemClicked;
    }

    protected override void DisconnectHandler(AppCompatAutoCompleteTextView platformView)
    {
        platformView.TextChanged -= PlatformView_TextChanged;
        platformView.FocusChange -= PlatformView_FocusChange;
        platformView.ItemClick -= PlatformView_ItemClicked;
    }

    private void PlatformView_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
    {
        if (VirtualView.Text != PlatformView.Text)
        {
            VirtualView.Text = PlatformView.Text;
        }
    }

    private void PlatformView_FocusChange(object sender, Android.Views.View.FocusChangeEventArgs e)
    {
        if (e.HasFocus && VirtualView.Threshold == 0)
        {
            PlatformView.ShowDropDown();
        }
    }
   
    private void PlatformView_ItemClicked(object sender, Android.Widget.AdapterView.ItemClickEventArgs e)
    {
        if (VirtualView.SelectedText != PlatformView.Text)
        {
            VirtualView.SelectedText = PlatformView.Text;
        }
    }

    private void SetItemsSource()
    {
        if (VirtualView.ItemsSource == null)
        {
            return;
        }

        ResetAdapter();
    }

    private void ResetAdapter()
    {
        var adapter = new BoxArrayAdapter(Context,
            Android.Resource.Layout.SimpleDropDownItem1Line,
            VirtualView.ItemsSource);

        PlatformView.Adapter = adapter;

        adapter.NotifyDataSetChanged();
    }

    public static void MapText(AutoCompleteViewHandler handler, AutoCompleteView view)
    {
        if (handler.PlatformView.Text != view.Text)
        {
            handler.PlatformView.Text = view.Text;
        }
    }

    public static void MapItemsSource(AutoCompleteViewHandler handler, AutoCompleteView view)
    {
        handler.SetItemsSource();
    }

    public static void MapThreshold(AutoCompleteViewHandler handler, AutoCompleteView view)
    {
        if (handler.PlatformView.Threshold != view.Threshold)
        {
            handler.PlatformView.Threshold = view.Threshold;
        }
    }
}

internal class BoxArrayAdapter : ArrayAdapter
{
    private readonly IList _objects;

    public BoxArrayAdapter(
        Context context,
        int textViewResourceId,
        IList objects) : base(context, textViewResourceId, objects)
    {
        _objects = objects;
    }
}

#endif