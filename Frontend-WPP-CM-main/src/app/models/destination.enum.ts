export enum Destination {
  Amsterdam = 'Nachtwachtlaan 20, 1058 EA Amsterdam',
  Arnhem = 'Velperweg 27, 6824 BC Arnhem',
  Breda = 'Konijnenberg 24, 4825 BD Breda',
  Enschede = 'Brouwerijstraat 1, 7523 XC Enschede',
  Maastricht = 'Randwycksingel 35, 6229 EG Maastricht',
  Utrecht = 'Vliegend Hertlaan 39, 3526 KT Utrecht',
  Other = 'other',
}

export const Destination2LabelMapping: Record<Destination, string> = {
  [Destination.Amsterdam]: 'Nachtwachtlaan 20, Amsterdam',
  [Destination.Arnhem]: 'Velperweg 27, Arnhem',
  [Destination.Breda]: 'Konijnenberg 24, Breda',
  [Destination.Enschede]: 'Brouwerijstraat 1, Enschede',
  [Destination.Maastricht]: 'Randwycksingel 35, Maastricht',
  [Destination.Utrecht]: 'Vliegend Hertlaan 39, Utrecht',
  [Destination.Other]: 'Other...',
};
