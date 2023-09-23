export class CurrentCourt {
  name: string = ''
  title: string = '';
  imgsSrc: string[] = [];
}

export const CurrentCourtCollection: CurrentCourt[] = [
  {
    name: 'Krystin Buist',
    title: 'Miss Bridgman',
    imgsSrc: [
      '../../assets/images/2019_MissBridgman.jpg',
      '../../assets/images/2019_MissBridgman2.jpg'
    ]
  },
  {
    name: 'Justin Jannert',
    title: 'Mr Nice Guy',
    imgsSrc: [ '../../assets/images/2019_MrNiceGuy.jpg']
  },
  {
    name: 'Dempsey DeMercado & LaShanna Sparks',
    title: 'Little Miss Bridgman',
    imgsSrc: [ '../../assets/images/2019_LittleMissBridgman.jpg']
  }
];
