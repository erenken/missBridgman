export class MenuItem {
  displayName: string = '';
  route: string = '';
}

export const MenuItems: MenuItem[] = [
  { displayName: 'Home', route: '/home' },
  { displayName: 'Who We Are', route: '/whoweare' },
  // { displayName: 'Sponsors', route: '/sponsors' },
  // { displayName: 'Photos', route: '/photos' },
  { displayName: 'Tickets', route: '/tickets' },
  { displayName: 'Forms', route:'/forms' },
  { displayName: 'Become A Contestant', route: '/becomecontestant' },
];
