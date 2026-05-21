public interface IInteractable
{
    void Interact();
    bool Caninteract();

    // Apakah berinteraksi langsung
    bool CanDirectInteract()
    {
        return false; // default: tidak bisa langsung berinteraksi
    }
}
