import { LinkItem } from "../common/LinkItem";

export class ContesantFormGroup {
    header: string = '';
    formItems: LinkItem[] = [];
}

var documentUrl = "../../assets/documents/contestant";

export const ContesantFormGroups:  ContesantFormGroup[] = [
    // { header: "Little Miss.", formItems: [ 
    //     { displayName: "Practice Schedule", link: `${documentUrl}/2023 LM JR Teen Teen Bridgman Practice Schedule.docx` },
    //     { displayName: "Contestant Application", link: `${documentUrl}/2023 Little Miss Contestant Application.docx` },
    //     { displayName: "Contestant Biography", link: `${documentUrl}/2023 Little Miss Contestant Biography.doc` }
    // ] },
    // { header: "Teen", formItems: [ 
    //     { displayName: "Practice Schedule", link: `${documentUrl}/2023 LM JR Teen Teen Bridgman Practice Schedule.docx` },
    //     { displayName: "Contestant Application", link: `${documentUrl}/2023 Junior Teen Contestant Application.docx` },
    //     { displayName: "Application", link: `${documentUrl}/2023 Bridgman Teen Application.docx` },
    //     { displayName: "Contestant Biography", link: `${documentUrl}/2023 Junior Teen Contestant Bio.docx` },
    //     { displayName: "Biography", link: `${documentUrl}/2023 Teen Bridgman Biography.docx` }
    // ] },
    { header: "Mr. Bridgman", formItems: [ 
        { displayName: "Official Application Packet", link: `${documentUrl}/2024 Blossomtime KING Official Application Packet.pdf` },
        // { displayName: "Bridgman Practice Schedule", link: `${documentUrl}/2023 MissMr Bridgman Practice Schedule.docx` },
        // { displayName: "Contestant Application", link: `${documentUrl}/2023 MissMr Bridgman Contestant Application.docx` }
    ] },
    { header: "Miss. Bridgman", formItems: [ 
        { displayName: "Official Application Packet", link: `${documentUrl}/2024 Blossomtime QUEEN Official Application Packet.pdf` },
        // { displayName: "Bridgman Practice Schedule", link: `${documentUrl}/2023 MissMr Bridgman Practice Schedule.docx` },
        // { displayName: "Contestant Application", link: `${documentUrl}/2023 MissMr Bridgman Contestant Application.docx` }
    ] }
];