import { LinkItem } from "../common/LinkItem";

export class ContesantFormGroup {
    header: string = '';
    formItems: LinkItem[] = [];
}

var documentUrl = "../../assets/documents/contestant";

export const ContesantFormGroups:  ContesantFormGroup[] = [
    { header: "Little Miss", formItems: [ 
        { displayName: "Little Miss Junior Teen Teen Bridgman Practice Schedule", link: `${documentUrl}/2023 LM JR Teen Teen Bridgman Practice Schedule.docx` },
        { displayName: "Little Miss Contestant Application", link: `${documentUrl}/2023 Little Miss Contestant Application.docx` },
        { displayName: "Little Miss Contestant Biography", link: `${documentUrl}/2023 Little Miss Contestant Biography.doc` }
    ] }
];