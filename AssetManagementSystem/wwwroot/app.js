document.addEventListener('DOMContentLoaded', function () {
    M.AutoInit();

    const assetForm = document.getElementById('assetForm');
    const assetList = document.getElementById('assetList');

    let assets = [];

    assetForm.addEventListener('submit', function (e) {
        e.preventDefault();

        const name = document.getElementById('assetName').value;
        const type = document.getElementById('assetType').value;
        const serial = document.getElementById('serialNumber').value;

        const asset = { name, type, serial };
        assets.push(asset);

        const li = document.createElement('li');
        li.className = 'collection-item';
        li.textContent = `${name} - ${type} (${serial})`;
        assetList.appendChild(li);

        assetForm.reset();
        M.updateTextFields();
    });
});
