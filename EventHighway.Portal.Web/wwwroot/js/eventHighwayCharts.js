// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

window.eventHighwayCharts = {
    _charts: {},

    _rgba: function (hex, alpha) {
        if (!hex || hex[0] !== "#" || hex.length < 7) {
            return "rgba(50, 31, 219, " + alpha + ")";
        }

        const r = parseInt(hex.substr(1, 2), 16);
        const g = parseInt(hex.substr(3, 2), 16);
        const b = parseInt(hex.substr(5, 2), 16);

        return "rgba(" + r + ", " + g + ", " + b + ", " + alpha + ")";
    },

    render: function (canvasId, config) {
        const element = document.getElementById(canvasId);

        if (!element || typeof Chart === "undefined") {
            return;
        }

        if (this._charts[canvasId]) {
            this._charts[canvasId].destroy();
        }

        const context = element.getContext("2d");
        const type = config.type;
        const self = this;

        (config.data.datasets || []).forEach(function (dataset) {
            if (type === "line") {
                if (dataset.fill) {
                    const gradient = context.createLinearGradient(0, 0, 0, element.height || 300);
                    gradient.addColorStop(0, self._rgba(dataset.borderColor, 0.35));
                    gradient.addColorStop(1, self._rgba(dataset.borderColor, 0));
                    dataset.backgroundColor = gradient;
                } else {
                    dataset.backgroundColor = self._rgba(dataset.borderColor, 0);
                }
            } else if (Array.isArray(dataset.backgroundColor) && dataset.backgroundColor.length === 1) {
                // A single-colour bar series — apply the one colour to every bar.
                dataset.backgroundColor = dataset.backgroundColor[0];
                dataset.borderColor = dataset.backgroundColor;
            }
        });

        this._charts[canvasId] = new Chart(element, config);
    },

    destroy: function (canvasId) {
        if (this._charts[canvasId]) {
            this._charts[canvasId].destroy();
            delete this._charts[canvasId];
        }
    }
};
