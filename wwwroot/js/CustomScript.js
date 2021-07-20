function confirmDelete(uniqueId, isDeleteClicked) {
    var deleteSpan = 'deleteSpan_' + uniqueId;
    var confirmDeletespan = 'confirmDeleteSpan_' + uniqueId;

    if (isDeleteClicked) {
        $('#' + deleteSpan).hide();
        $('#' + confirmDeletespan).show();
    } else {
        $('#' + deleteSpan).show();
        $('#' + confirmDeletespan).hide();
    }
}
