function DownloadExcel(nameFile, base64File) {
    const link = document.createElement("a");
    link.download = nameFile;
    link.href = "data:application/vnd.ms-excel;base64," + base64File;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}