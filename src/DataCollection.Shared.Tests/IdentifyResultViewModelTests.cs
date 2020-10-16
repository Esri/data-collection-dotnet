/*******************************************************************************
  * Copyright 2020 Esri
  *
  *  Licensed under the Apache License, Version 2.0 (the "License");
  *  you may not use this file except in compliance with the License.
  *  You may obtain a copy of the License at
  *
  *  http://www.apache.org/licenses/LICENSE-2.0
  *
  *   Unless required by applicable law or agreed to in writing, software
  *   distributed under the License is distributed on an "AS IS" BASIS,
  *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  *   See the License for the specific language governing permissions and
  *   limitations under the License.
******************************************************************************/

using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.Models;
using Esri.ArcGISRuntime.OpenSourceApps.DataCollection.Shared.ViewModels;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DataCollection.Shared.Tests
{
    /// <summary>
    /// Tests for the <see cref="IdentifyResultViewModel" />.
    /// </summary>
    [TestFixture]
    public class IdentifyResultViewModelTests
    {
        // This is needed to construct identified features; in the future that dependency should be removed
        private MainViewModel _mainViewModel;
        private List<IdentifiedFeatureViewModel> _sharedResultList;

        [SetUp]
        public void SetUpTests()
        {
            _mainViewModel = new MainViewModel();
            _sharedResultList = new List<IdentifiedFeatureViewModel>();
        }

        /// <summary>
        /// Checks that <see cref="IdentifyResultViewModel.ResultCount" /> returns a valid value for various scenarios.
        /// </summary>
        /// <param name="resultCount">Number of identified results to test with.</param>
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void GettingResultCountDoesNotThrow(int resultCount)
        {
            // Set up test
            AddDummyResults(resultCount);
            IdentifyResultViewModel identifyVM = new IdentifyResultViewModel();

            Assert.AreEqual(identifyVM.ResultCount, 0);

            identifyVM.SetNewIdentifyResult(_sharedResultList);
            Assert.AreEqual(identifyVM.ResultCount, resultCount);

            identifyVM.SetNewIdentifyResult(null);
            Assert.AreEqual(identifyVM.ResultCount, 0);
        }

        /// <summary>
        /// Verifies that the first feature is selected when a list containing only one item is set.
        /// </summary>
        [TestCase]
        public void FirstResultSelectedWhenListHasOneItem()
        {
            var baseResultsList = new List<IdentifiedFeatureViewModel>();
            var singleResult = new IdentifiedFeatureViewModel(null, ConnectivityMode.Online, _mainViewModel);
            baseResultsList.Add(singleResult);

            IdentifyResultViewModel _identifyVM = new IdentifyResultViewModel();
            _identifyVM.SetNewIdentifyResult(baseResultsList);

            Assert.AreEqual(_identifyVM.CurrentlySelectedFeature, singleResult);
        }

        /// <summary>
        /// Verifies that feature selection is reset when a new list is set.
        /// </summary>
        [TestCase]
        public void FeatureSelectionResetWhenNewListAssigned()
        {
            // Set up test
            AddDummyResults(6);

            IdentifyResultViewModel identifyVM = new IdentifyResultViewModel();
            identifyVM.SetNewIdentifyResult(_sharedResultList);

            Assert.IsNull(identifyVM.CurrentFeatureIndex);
            Assert.IsNull(identifyVM.CurrentlySelectedFeature);

            identifyVM.CurrentFeatureIndex = 1;

            Assert.IsNotNull(identifyVM.CurrentFeatureIndex);
            Assert.IsNotNull(identifyVM.CurrentlySelectedFeature);

            // re-create list
            _sharedResultList = new List<IdentifiedFeatureViewModel>();
            AddDummyResults(6);

            // Verify selection is reset
            identifyVM.SetNewIdentifyResult(_sharedResultList);
            Assert.IsNull(identifyVM.CurrentFeatureIndex);
            Assert.IsNull(identifyVM.CurrentlySelectedFeature);
        }

        /// <summary>
        /// Verifies that PCE is raised only when it should be.
        /// </summary>
        [TestCase]
        public void AllEventsAreRaisedWhenValuesChange()
        {
            // Set up test
            AddDummyResults(1);
            IdentifyResultViewModel identifyVM = new IdentifyResultViewModel();

            List<string> changedProperties = new List<string>();
            PropertyChangedEventHandler rememberingEventHandler = delegate (object sender, PropertyChangedEventArgs e)
            {
                changedProperties.Add(e.PropertyName);
            };
            identifyVM.PropertyChanged += rememberingEventHandler;

            // Set a new result
            identifyVM.SetNewIdentifyResult(_sharedResultList);

            // List should contain all properties, since setting a list with one item automatically selects that item
            Assert.Contains(nameof(identifyVM.IdentifiedFeatures), changedProperties);
            Assert.Contains(nameof(identifyVM.CurrentFeatureIndex), changedProperties);
            Assert.Contains(nameof(identifyVM.CurrentlySelectedFeature), changedProperties);
            Assert.Contains(nameof(identifyVM.ResultCount), changedProperties);

            // Clear existing results and set new list with multiple items
            changedProperties.Clear();
            _sharedResultList = new List<IdentifiedFeatureViewModel>();
            AddDummyResults(6);

            identifyVM.SetNewIdentifyResult(_sharedResultList);

            // List should contain all properties, since clearing selection should unselect the previously selected single feature
            Assert.Contains(nameof(identifyVM.IdentifiedFeatures), changedProperties);
            Assert.Contains(nameof(identifyVM.ResultCount), changedProperties);
            Assert.Contains(nameof(identifyVM.CurrentFeatureIndex), changedProperties);
            Assert.Contains(nameof(identifyVM.CurrentlySelectedFeature), changedProperties);

            // Clear existing results and set new list with multiple items
            changedProperties.Clear();
            _sharedResultList = new List<IdentifiedFeatureViewModel>();
            AddDummyResults(8);

            identifyVM.SetNewIdentifyResult(_sharedResultList);

            Assert.Contains(nameof(identifyVM.IdentifiedFeatures), changedProperties);
            Assert.Contains(nameof(identifyVM.ResultCount), changedProperties);
            Assert.False(changedProperties.Contains(nameof(identifyVM.CurrentFeatureIndex)));
            Assert.False(changedProperties.Contains(nameof(identifyVM.CurrentlySelectedFeature)));
        }

        /// <summary>
        /// Verifies that setting the selected feature index to null clears feature selection.
        /// </summary>
        /// <param name="resultCount">Number of identified results to test with.</param>
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void SettingNullFeatureIndexClearsSelection(int resultCount)
        {
            // Set up test
            AddDummyResults(resultCount);
            IdentifyResultViewModel identifyVM = new IdentifyResultViewModel();
            identifyVM.SetNewIdentifyResult(_sharedResultList);

            if (resultCount > 0)
            {
                identifyVM.CurrentFeatureIndex = resultCount - 1;
                Assert.AreEqual(identifyVM.CurrentlySelectedFeature, _sharedResultList.ElementAt(resultCount - 1));
            }

            identifyVM.CurrentFeatureIndex = null;
            Assert.IsNull(identifyVM.CurrentlySelectedFeature);
        }

        /// <summary>
        /// Verifies that exceptions are thrown when invalid feature index is set.
        /// </summary>
        /// <param name="resultCount">Number of identified results to test with.</param>
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void SetInvalidFeatureIndex(int resultCount)
        {
            // Set up test
            AddDummyResults(resultCount);
            IdentifyResultViewModel identifyVM = new IdentifyResultViewModel();
            identifyVM.SetNewIdentifyResult(_sharedResultList);

            // -1 can't throw because that breaks UWP
            Assert.DoesNotThrow(() =>
            {
                identifyVM.CurrentFeatureIndex = -1;
                Assert.AreEqual(identifyVM.CurrentFeatureIndex, null);
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                identifyVM.CurrentFeatureIndex = resultCount;
            });

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                identifyVM.CurrentFeatureIndex = resultCount + 1;
            });
        }

        /// <summary>
        /// Checks that the first feature is selected when a new result list containing exactly one feature is set.
        /// </summary>
        /// <param name="resultCount">Number of identified results to test with.</param>
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void AutomaticFeatureSelectionForSingleElementList(int resultCount)
        {
            // Set up test
            AddDummyResults(resultCount);
            IdentifyResultViewModel identifyVM = new IdentifyResultViewModel();

            Assert.IsNull(identifyVM.CurrentFeatureIndex);
            Assert.IsNull(identifyVM.CurrentlySelectedFeature);

            identifyVM.SetNewIdentifyResult(null);

            Assert.AreEqual(identifyVM.ResultCount, 0);
            Assert.IsNull(identifyVM.CurrentFeatureIndex);
            Assert.IsNull(identifyVM.CurrentlySelectedFeature);

            identifyVM.SetNewIdentifyResult(_sharedResultList);

            if (resultCount != 1)
            {
                Assert.IsNull(identifyVM.CurrentFeatureIndex);
                Assert.IsNull(identifyVM.CurrentlySelectedFeature);
            }
            else
            {
                Assert.AreEqual(identifyVM.CurrentFeatureIndex, 0);
                Assert.AreEqual(identifyVM.CurrentlySelectedFeature, _sharedResultList.First());
            }
        }

        /// <summary>
        /// Verifies the behavior of <see cref="IdentifyResultViewModel.PreviousResultCommand" />.
        /// </summary>
        /// <param name="resultCount">Number of identified results to test with.</param>
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void VerifyPreviousResultWrapAround(int resultCount)
        {
            // Set up test
            AddDummyResults(resultCount);
            IdentifyResultViewModel identifyVM = new IdentifyResultViewModel();

            // Before results are set; command shouldn't throw exception if called early
            identifyVM.PreviousResultCommand.Execute(null);
            Assert.AreEqual(identifyVM.CurrentFeatureIndex, null);
            Assert.AreEqual(identifyVM.CurrentlySelectedFeature, null);

            // After results are set, prev result shouldn't do anything until a selection is made
            // A default selection is made for the case of one result only
            identifyVM.SetNewIdentifyResult(_sharedResultList);

            identifyVM.PreviousResultCommand.Execute(null);
            if (resultCount != 1)
            {
                Assert.IsNull(identifyVM.CurrentFeatureIndex);
                Assert.IsNull(identifyVM.CurrentlySelectedFeature);
            }

            // Verify wrap around
            if (resultCount > 0)
            {
                identifyVM.CurrentFeatureIndex = 0;
            }
            for (int x = 0; x < resultCount * 2; x++)
            {
                identifyVM.PreviousResultCommand.Execute(null);
                Assert.NotNull(identifyVM.CurrentlySelectedFeature);
                Assert.NotNull(identifyVM.CurrentFeatureIndex);
                Assert.GreaterOrEqual(identifyVM.CurrentFeatureIndex, 0);
                Assert.Less(identifyVM.CurrentFeatureIndex, resultCount);
            }
        }

        /// <summary>
        /// Verifies that <see cref="IdentifyResultViewModel.NextResultCommand" /> works properly.
        /// </summary>
        /// <param name="resultCount">Number of identified results to test with.</param>
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void VerifyNextResultWraparound(int resultCount)
        {
            // Set up the test
            AddDummyResults(resultCount);
            IdentifyResultViewModel identifyVM = new IdentifyResultViewModel();

            // Before results are set; command should not throw exception if called before any value is set
            identifyVM.NextResultCommand.Execute(null);
            Assert.IsNull(identifyVM.CurrentFeatureIndex);
            Assert.IsNull(identifyVM.CurrentlySelectedFeature);

            identifyVM.SetNewIdentifyResult(_sharedResultList);

            // After results are set, next result shouldn't do anything until a selection is made
            // A default selection is made for the case of one result only
            identifyVM.NextResultCommand.Execute(null);
            if (resultCount != 1)
            {
                Assert.IsNull(identifyVM.CurrentFeatureIndex);
                Assert.IsNull(identifyVM.CurrentlySelectedFeature);
            }

            // Verify wrap around
            if (resultCount > 0)
            {
                identifyVM.CurrentFeatureIndex = 0;
            }
            for (int x = 0; x < resultCount * 2; x++)
            {
                identifyVM.NextResultCommand.Execute(null);
                Assert.NotNull(identifyVM.CurrentlySelectedFeature);
                Assert.NotNull(identifyVM.CurrentFeatureIndex);
                Assert.GreaterOrEqual(identifyVM.CurrentFeatureIndex, 0);
                Assert.Less(identifyVM.CurrentFeatureIndex, resultCount);
            }
        }

        /// <summary>
        /// Verifies that clearing results works properly.
        /// </summary>
        /// <param name="resultCount">Number of identified results to test with.</param>
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void ClearResultsDoesNotThrow(int resultCount)
        {
            // Set up test
            AddDummyResults(resultCount);
            IdentifyResultViewModel identifyVM = new IdentifyResultViewModel();

            // Clear before setting
            identifyVM.ClearResultsCommand.Execute(null);

            // Clear after setting, verify cleared
            Assert.IsNull(identifyVM.IdentifiedFeatures);
            identifyVM.SetNewIdentifyResult(_sharedResultList);
            Assert.AreEqual(identifyVM.IdentifiedFeatures, _sharedResultList);
            Assert.AreEqual(identifyVM.ResultCount, resultCount);

            identifyVM.ClearResultsCommand.Execute(null);
            Assert.IsNull(identifyVM.IdentifiedFeatures);
            Assert.AreEqual(identifyVM.ResultCount, 0);
            Assert.IsNull(identifyVM.CurrentFeatureIndex);
        }

        /// <summary>
        /// Verifies that clearing selection works properly in various scenarios.
        /// </summary>
        /// <param name="resultCount">Number of identified results to test with.</param>
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void ClearSelectionDoesNotThrow(int resultCount)
        {
            // Set up test
            AddDummyResults(resultCount);
            IdentifyResultViewModel identifyVM = new IdentifyResultViewModel();

            // Verify selection can be cleared under a variety of circumstances
            identifyVM.ClearSelectionCommand.Execute(null);

            identifyVM.SetNewIdentifyResult(_sharedResultList);

            identifyVM.ClearSelectionCommand.Execute(null);

            Assert.IsNull(identifyVM.CurrentFeatureIndex);
            Assert.IsNull(identifyVM.CurrentlySelectedFeature);

            if (resultCount > 0)
            {
                identifyVM.CurrentFeatureIndex = 0;
            }

            identifyVM.ClearSelectionCommand.Execute(null);
            
            Assert.IsNull(identifyVM.CurrentFeatureIndex);
            Assert.IsNull(identifyVM.CurrentlySelectedFeature);
        }

        /// <summary>
        /// Sets up the input list of identify results.
        /// </summary>
        /// <param name="resultCount">Count of entries to add</param>
        private void AddDummyResults(int resultCount)
        {
            for (int x = 0; x < resultCount; x++)
            {
                _sharedResultList.Add(new IdentifiedFeatureViewModel(null, ConnectivityMode.Online, _mainViewModel));
            }
        }
    }
}
