export class MenuItem {
  displayName: string = '';
  route: string = '';
}

export const MenuItems: MenuItem[] = [
  { displayName: 'Home', route: '/home' },
  // { displayName: 'Photos', route: '/photos' },
  { displayName: 'Documents & Forms', route: '/documents' },
  { displayName: 'Who We Are', route: '/whoweare' }
];
