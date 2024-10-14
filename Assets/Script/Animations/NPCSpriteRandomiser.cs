using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class NPCSpriteRandomiser : MonoBehaviour
{

    [SerializeField] private SpriteResolver _rightHandSpriteResolver;
    [SerializeField] private SpriteResolver _leftHandSpriteResolver;
    [SerializeField] private SpriteResolver _hairSpriteResolver;
    [SerializeField] private SpriteResolver _headSpriteResolver;
    [SerializeField] private SpriteResolver _bodySpriteResolver;
    [SerializeField] private SpriteResolver _tieSpriteResolver;

    const string RIGHT_HAND = "RightHand";
    const string LEFT_HAND = "LeftHand";
    const string HAIR = "HairStyle";
    const string HEAD = "HeadColor";
    const string SHIRT = "Body";
    const string TIE = "Tie";

    [SerializeField] private SpriteLibraryAsset _spriteLibraryAsset;


    // Start is called before the first frame update

    void Start()
    {
        RandomiseNpcVisuals();
    }

    private void RandomiseNpcVisuals()
    {
        RandomiseHairStyle(_hairSpriteResolver, HAIR);
        RandomiseSkin();
    }

    private void RandomiseSkin()
    {
        var labelsList = _spriteLibraryAsset.GetCategoryLabelNames(HEAD).ToList();
        int randomIndex = UnityEngine.Random.Range(0, labelsList.Count);
        _rightHandSpriteResolver.SetCategoryAndLabel(RIGHT_HAND, labelsList[randomIndex]);
        _leftHandSpriteResolver.SetCategoryAndLabel(LEFT_HAND, labelsList[randomIndex]);
        _headSpriteResolver.SetCategoryAndLabel(HEAD, labelsList[randomIndex]);
        Debug.LogWarning("Randomised skin: " + labelsList[randomIndex]);
    }

    private void RandomiseHairStyle(SpriteResolver hairSpriteResolver, string hair)
    {
        var labelsList = _spriteLibraryAsset.GetCategoryLabelNames(hair).ToList();

        int randomIndex = UnityEngine.Random.Range(0, labelsList.Count);

        hairSpriteResolver.SetCategoryAndLabel(hair, labelsList[randomIndex]);

        Debug.Log("Randomised hair style: " + labelsList[randomIndex]);
    }

    
}
