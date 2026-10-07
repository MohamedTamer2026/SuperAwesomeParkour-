using UnityEngine;

public class CharacterSwitcher : MonoBehaviour
{
    public GameObject[] characterPrefabs;
    public GameObject currentCharacter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SwitchCharacter(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchCharacter(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchCharacter(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchCharacter(2);
    }

    public void SwitchCharacter(int index)
    {

        if (index < 0 || index >= characterPrefabs.Length) return;

        if (currentCharacter != null) Destroy(currentCharacter);

        currentCharacter = Instantiate(characterPrefabs[index], transform.position, transform.rotation, transform);
        currentCharacter.transform.localPosition = new Vector3(0, 0f, 0);
        Animator anim = currentCharacter.GetComponentInChildren<Animator>();

        if (anim != null)
        {
            GetComponent<SimplePlayerController>()?.SetAnimator(anim);
        }
    }
}
