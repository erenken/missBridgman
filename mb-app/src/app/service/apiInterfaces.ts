export interface Folder {
    Folders: {
        [key: string]: Folder;
    } | null;
    Files: File[];
}

export interface File {
    Name: string;
    Description: string;
    Url: string;
}