// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

window.eventHighwayCharts = {
    _charts: {},

    render: function (canvasId, config) {
        const element = document.getElementById(canvasId);

        if (!element || typeof Chart === "undefined") {
            return;
        }

        if (this._charts[canvasId]) {
            this._charts[canvasId].destroy();
        }

        this._charts[canvasId] = new Chart(element, config);
    },

    destroy: function (canvasId) {
        if (this._charts[canvasId]) {
            this._charts[canvasId].destroy();
            delete this._charts[canvasId];
        }
    }
};
