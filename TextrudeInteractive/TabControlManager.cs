﻿using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace TextrudeInteractive
{
    public class TabControlManager<T> where T : class, IPane, new()
    {
        private readonly PaneCache<T> _cache;
        private readonly string _prefix;
        private readonly TabControl _tab;
        public List<T> Panes = new();

        public TabControlManager(string prefix, TabControl tab, Action<T> onNewPane)
        {
            _prefix = prefix;
            _tab = tab;
            _cache = new PaneCache<T>(onNewPane);
        }


        public T AddPane()
        {
            var currentCount = Panes.Count;
            var pane = _cache.Obtain();
            Panes.Add(pane);

            _tab.Items.Add(
                new TabItem
                {
                    Content = pane,
                    Header = $"{_prefix}{currentCount}"
                });
            _tab.SelectedIndex = _tab.Items.Count - 1;
            return pane;
        }

        public void RemoveLast()
        {
            if (_tab.Items.Count == 0)
                return;
            var last = _tab.Items[^1] as TabItem;
            _tab.Items.Remove(last);
            var pane = last.Content as T;
            Panes.Remove(pane);
            _cache.Release(pane);
        }

        public void Clear()
        {
            while (Panes.Count > 0)
                RemoveLast();
        }

        public void ForAll(Action<T> func)
        {
            foreach (var p in Panes)
                func(p);
        }
    }
}