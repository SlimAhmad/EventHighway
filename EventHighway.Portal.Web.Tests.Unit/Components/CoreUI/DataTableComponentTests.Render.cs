// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using Bunit;
using EventHighway.Portal.Web.Components.CoreUI;
using FluentAssertions;

namespace EventHighway.Portal.Web.Tests.Unit.Components.CoreUI
{
    public partial class DataTableComponentTests
    {
        [Fact]
        public void ShouldInitializeComponent()
        {
            // given
            List<Sample> samples = CreateSamples(count: 3);
            List<DataTableColumn<Sample>> columns = CreateColumns();

            // when
            IRenderedComponent<DataTable<Sample>> renderedDataTable =
                Render<DataTable<Sample>>(parameters => parameters
                    .Add(dataTable => dataTable.Items, samples)
                    .Add(dataTable => dataTable.Columns, columns));

            // then
            renderedDataTable.Instance.CurrentPage.Should().Be(1);
            renderedDataTable.Instance.SearchTerm.Should().BeEmpty();
            renderedDataTable.Instance.SortColumn.Should().BeNull();
        }
    }
}
