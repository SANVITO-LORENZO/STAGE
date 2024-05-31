using interfaces;


veleno veleno = new veleno();
HandlePick(veleno);

spada spada = new spada();
HandlePick(spada);


void HandlePick(IPickup pickup)
{
    pickup.Pickup();
}