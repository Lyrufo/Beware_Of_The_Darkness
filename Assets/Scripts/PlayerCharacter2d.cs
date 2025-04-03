using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]

// force à mettre ces composants

public class PlayerCharacter2D : MonoBehaviour
{

    [Tooltip("la 'difficulté' pour faire avancer le perso (sa vitesse d'accélération)")]
    public float movementAcceleration = 20f;

    [Tooltip("vitesse max/s sur X")]
    public float maxMovementSpeed = 4f;

    [Tooltip("la 'difficulté' pour faire sauter le perso (force appliquée)")]
    public float jumpForce = 16f;

    [Tooltip("le layer sur lequel ça agit pour éviter que le chara capte son propre collider ")]
    public LayerMask groundLayer;

    [Tooltip("la taille du 'rayon' raycast si on veut sauter à nouveau sans toucher le sol")]
    public float groundDistance = 1.1f;

    private Rigidbody2D _rigidbody = null; //pour bouger

    private Animator _animator = null; //pour mettre les anim

    private SpriteRenderer _spriteRenderer = null; // pour montrer le chara sur l'écran
 

    // = null c'est pour etre sur que en gros ça fait ref à rien dès le départ


    private void Awake()
    {
        //Void awake pour être lancé avant un quelconque start => ici pour choper les coponents direct et forcer le fait qu'ils soient sur l'objet où le script est mis
        _rigidbody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

    }


    private void Update()
    //à chaque frame on va appeler les 4 fonctions en dessous là
    {
        UpdateMovement(); //met à jour le sens de l'anim 
        UpdateJump(); //vérifie si on appuie espace pour sauter
        ClampVelocity(); //capte la vitesse pour limiter la vitesse max
        UpdateGround(); //vérifie si le joueur est bien au sol

        //ces lignes c'est dédié à l'aimation et à laquelle jouer selon la vitesse et l'état du perso

        // Quand la vitesse en X est négative, on inverse l'animation pour regarder à gauche

        if (_rigidbody.velocity.x > 0) //qd va vers la droite
        {
            _spriteRenderer.flipX = false; //ne flip pas
        }
        else if (_rigidbody.velocity.x < 0) //qd va vers la gauche
        {
            _spriteRenderer.flipX = true; // flip
        }

        //mais tout ça dans le cas où à la base il était vers la droite, puisqu'on se base sur la vélocité


        // Utilise la vitesse du personnage pour choisir l'animation ("idle", "move", etc.)
        _animator.SetFloat("xVelocity", Mathf.Abs(_rigidbody.velocity.x));


        // Si la vitesse en Y est positive, on dit que le personnage saute, sinon il tombe.
        _animator.SetBool("isJumping", _rigidbody.velocity.y > 0);
        _animator.SetBool("isFalling", _rigidbody.velocity.y < 0);
    }


    private void UpdateMovement()
    {
        float xMovement = 0f; //initiatlise le mvt du perso à 0 sur x

        if (Input.GetKey(KeyCode.RightArrow))
        {
            xMovement += 1; //si fleche droite pressée -> +1 sur l'axe X donc avance vers droite
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            xMovement += -1; //si fleche gauche pressée -> -1 sur l'axe X donc avance vers gauche
        }

        if (xMovement == 0) //si perso bouge pas
        {
            _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y); // on met son mvt sur x à 0 mais on laisse son état sur y comme il est (donc s'il saute ou tombe)
        }
        else //se joue ds tous les cas dès qu'il y a un mvt (lorsque xmovement =/ 0) donc ds tous si une key est pressée
        {
            _rigidbody.velocity += Vector2.right * xMovement * movementAcceleration * Time.deltaTime;
            //ok donc ici, il s'agit d'appliquer la vitesse selon la physique (la direction et le mvt a déjà été défini plus haut)
            //velocity c'est un composant de rigidbody qui représente la vitesse (sur x et y) et donc ici on va y ajouter un mvt 
            //vector2 c'est un vecteur 2d auquel on peut donner un x et y entre -1 et 1 (x:y) mais en ajoutant .right on lock le y à 0 et le x à 1
            //xmovement peut stocker 1 ou -1 donc c'est pour inverser ou non le résultat en négatif si vers la gauche car on a bloqué x en postitif en ajouter right à vector2
            //movement acceleration c'est la variable qu'on a déterminé tout en haut (donc on peut direct écrire 20f en théorie dans la fonction) et ici c'est la vitessse d'accélération donc
            //Time.deltaTime notre meilleur pote en gros c'est le temps entre 2 frames (ici aussi en théorie on pourrait une valeur à la place, sauf qu'en mettant la fonction instead, ça s'adapt au nb de fps)

        }

    }


    private bool isGrounded = false; //booléen pour savoir si je touche le sol ou pas


    private void UpdateGround()
    {
        
        RaycastHit2D hit = Physics2D.CircleCast (transform.position, 0.2f,  Vector2.down, groundDistance, groundLayer);
        //Raycast envoie un rayon pour capter si y'a un collider en partant du bas de l'objet auquel on a assigné le script

        if (hit.collider != null) //si pas null alors a touché qqc, donc est au sol, donc je peux sauter
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
    private void OnDrawGizmos()
    {
        // "update" uniquement vu en mode editor
        // Ici pour voir la taille du Raycast
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundDistance);
        // en gros : tracer une ligne du centre l'objet (transform.position c'est les coordoonées basées sur le centre)  vers la position actuelle + la direction d'un vecteur (ici 3 car a du mal avec 2d) donc vers le bas de la taille grounddistance
    }

    private void UpdateJump()
    {

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded == true)  //DOWN nécessaire contrairement au dépacement car sinon se joue en boucle et is grounded pour être sûr d'être au sol
            {
                _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);


            //donc qd espace pressé : on prend l'objet, on lui applique une force (pas un mvt parce que c'est juste une impulsion sur y) 
            //vector2.up expliqué plus haut * la variable force définie en haut 
            //forcemode2d.impulse appplique direct en mode impulsion
            isGrounded = false; 
            //on remet à l'état faux jusqu'à ce que ça capte le sol et revien en true
            }
            
      
        }
    
    




    private void ClampVelocity() //pour vérifier qu'on dépasse pas la vitesse max sur x dont on a définit la variable tt en haut 
    {
        Vector2 velocity = _rigidbody.velocity; //on chope la valeur de la vélocité (que le x) actuelle 

        velocity.x = Mathf.Clamp(velocity.x, -maxMovementSpeed, maxMovementSpeed);
        //peu importe cette valeur elle doit être égale à ... = entre les deux valeurs (le max ou son inverse si on va vers la gauche) et si c'est le cas on lui assigne le max

        _rigidbody.velocity = velocity;
        //et pouf on lui applique cette valeur 
    }


}



