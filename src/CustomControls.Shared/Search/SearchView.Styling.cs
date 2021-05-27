using System;
using System.Collections.Generic;
using System.Text;
#if __WPF__
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace Esri.ArcGISRuntime.OpenSourceApps.DataCollection.CustomControls.Search
{
    public partial class SearchView
    {
        public Style GoButtonStyle
        {
            get { return (Style)GetValue(GoButtonStyleProperty); }
            set { SetValue(GoButtonStyleProperty, value); }
        }

        public Style SourceSelectButtonStyle
        {
            get { return (Style)GetValue(SourceSelectButtonStyleProperty); }
            set { SetValue(SourceSelectButtonStyleProperty, value); }
        }

        public Style ClearButtonStyle
        {
            get { return (Style)GetValue(ClearButtonStyleProperty); }
            set { SetValue(ClearButtonStyleProperty, value); }
        }

        public Style PlaceholderTextBlockStyle
        {
            get { return (Style)GetValue(PlaceholderTextBlockStyleProperty); }
            set { SetValue(PlaceholderTextBlockStyleProperty, value); }
        }

        public Style QueryTextBoxStyle
        {
            get { return (Style)GetValue(QueryTextBoxStyleProperty); }
            set { SetValue(QueryTextBoxStyleProperty, value); }
        }

        public DataTemplate SearchSuggestionTemplate
        {
            get { return (DataTemplate)GetValue(SearchSuggestionTemplateProperty); }
            set { SetValue(SearchSuggestionTemplateProperty, value); }
        }

        public DataTemplate SearchResultTemplate
        {
            get { return (DataTemplate)GetValue(SearchResultTemplateProperty); }
            set { SetValue(SearchResultTemplateProperty, value); }
        }

        public Style SuggestionPopupStyle
        {
            get { return (Style)GetValue(SuggestionPopupStyleProperty); }
            set { SetValue(SuggestionPopupStyleProperty, value); }
        }

        public Style SearchBarBorderStyle
        {
            get { return (Style)GetValue(SearchBarBorderStyleProperty); }
            set { SetValue(SearchBarBorderStyleProperty, value); }
        }

        public static readonly DependencyProperty SourceSelectButtonStyleProperty =
            DependencyProperty.Register("SourceSelectButtonStyle", typeof(Style), typeof(SearchView), new PropertyMetadata(null));
        public static readonly DependencyProperty ClearButtonStyleProperty =
            DependencyProperty.Register("ClearButtonStyle", typeof(Style), typeof(SearchView), new PropertyMetadata(null));
        public static readonly DependencyProperty PlaceholderTextBlockStyleProperty =
            DependencyProperty.Register("PlaceholderTextBlockStyle", typeof(Style), typeof(SearchView), new PropertyMetadata(null));
        public static readonly DependencyProperty QueryTextBoxStyleProperty =
            DependencyProperty.Register("QueryTextBoxStyle", typeof(Style), typeof(SearchView), new PropertyMetadata(null));
        public static readonly DependencyProperty SearchSuggestionTemplateProperty =
            DependencyProperty.Register("SearchSuggestionTemplate", typeof(DataTemplate), typeof(SearchView), new PropertyMetadata(null));
        public static readonly DependencyProperty SearchResultTemplateProperty =
            DependencyProperty.Register("SearchResultTemplate", typeof(DataTemplate), typeof(SearchView), new PropertyMetadata(null));
        public static readonly DependencyProperty SuggestionPopupStyleProperty =
            DependencyProperty.Register("SuggestionPopupStyle", typeof(Style), typeof(SearchView), new PropertyMetadata(null));
        public static readonly DependencyProperty SearchBarBorderStyleProperty =
            DependencyProperty.Register("SearchBarBorderStyle", typeof(Style), typeof(SearchView), new PropertyMetadata(null));
        public static readonly DependencyProperty GoButtonStyleProperty =
            DependencyProperty.Register("GoButtonStyle", typeof(Style), typeof(SearchView), new PropertyMetadata(null));
    }
}
