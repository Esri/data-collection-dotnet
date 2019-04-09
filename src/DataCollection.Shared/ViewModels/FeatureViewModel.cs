using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Extensions;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Messengers;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Models;
using Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Properties;
using Esri.ArcGISRuntime.Mapping.Popups;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.ViewModels
{
    public class FeatureViewModel : BaseViewModel
    {


        private PopupManager _popupManager;

        /// <summary>
        /// Gets or sets the active related record for the origin relationship
        /// </summary>
        public PopupManager PopupManager
        {
            get { return _popupManager; }
            set
            {
                if (_popupManager != value)
                {
                    _popupManager = value;
                    if (value != null)
                    {
                        Fields = FieldContainer.GetFields(value);

                        // If the selected related record changes, fetch the attachments and create a new AttachmentsViewModel
                        PopupManager.AttachmentManager.FetchAttachmentsAsync().ContinueWith(t =>
                        {
                            AttachmentsViewModel = new AttachmentsViewModel(PopupManager, FeatureTable);
                        });
                    }
                    OnPropertyChanged();
                }
            }
        }

        private AttachmentsViewModel _attachmentsViewModel;

        /// <summary>
        /// Gets or sets the AttachmentViewModel to handle viewing and editing attachments 
        /// </summary>
        public AttachmentsViewModel AttachmentsViewModel
        {
            get => _attachmentsViewModel;
            set
            {
                _attachmentsViewModel = value;
                OnPropertyChanged();
            }
        }

        private ArcGISFeatureTable _featureTable;

        /// <summary>
        /// Gets or sets the RelatedTable 
        /// </summary>
        public ArcGISFeatureTable FeatureTable
        {
            get => _featureTable;
            set
            {
                _featureTable = value;
                OnPropertyChanged();
            }
        }

        private IEnumerable<FieldContainer> _fields;

        /// <summary>
        /// Gets the underlying Field property for the PopupField in order to retrieve FieldType and Domain
        /// This is a workaroud until Domain and FieldType are exposed on the PopupManager
        /// </summary>
        public IEnumerable<FieldContainer> Fields
        {
            get => _fields;
            set
            {
                _fields = value;
                OnPropertyChanged();
            }
        }

        private Feature _feature;

        /// <summary>
        /// Gets or sets the feature currently selected
        /// </summary>
        public Feature Feature {
            get => _feature;
            set
            {
                _feature = value;
                OnPropertyChanged();
            }
        }

        private ConnectivityMode _connectivityMode;

        /// <summary>
        /// Gets or sets the ConnectivityMode
        /// </summary>
        public ConnectivityMode ConnectivityMode {
            get => _connectivityMode;

            set
            {
                _connectivityMode = value;
                OnPropertyChanged();
            }
        }

        private EditViewModel _editViewModel;

        /// <summary>
        /// Gets or sets the viewmodel for the current edit session
        /// </summary>
        public EditViewModel EditViewModel
        {
            get => _editViewModel;
            set
            {
                if (_editViewModel != value)
                {
                    _editViewModel = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Deletes identified feature
        /// </summary>
        internal async Task<bool> DeleteFeature()
        {
            if (Feature != null)
            {
                try
                {
                    await FeatureTable?.DeleteFeature(Feature);
                    await FeatureTable?.ApplyEdits();

                    return true;
                }
                catch (Exception ex)
                {
                    UserPromptMessenger.Instance.RaiseMessageValueChanged(null, ex.Message, true, ex.StackTrace);
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Discards edits performed on a feature 
        /// </summary>
        internal async Task<bool> DiscardChanges()
        {
            if (PopupManager.HasEdits())
            {
                bool cancelEdits = false;

                // wait for response from the user if the truly want to cancel the edit operation
                UserPromptMessenger.Instance.ResponseValueChanged += handler;

                UserPromptMessenger.Instance.RaiseMessageValueChanged(
                    Resources.GetString("DiscardEditsConfirmation_Title"),
                    Resources.GetString("DiscardEditsConfirmation_Message"),
                    false,
                    null,
                    Resources.GetString("DiscardButton_Content"));

                void handler(object o, UserPromptResponseChangedEventArgs e)
                {
                    {
                        UserPromptMessenger.Instance.ResponseValueChanged -= handler;
                        if (e.Response)
                        {
                            cancelEdits = true;
                        }
                    }
                }

                if (!cancelEdits)
                {
                    return false;
                }
            }

            // cancel the edits if the PopupManager doesn't have any edits or if the user chooses to
            EditViewModel.CancelEdits(PopupManager);
            EditViewModel = null;
            await AttachmentsViewModel.LoadAttachments();
            return true;
        }

    }
}
