export interface UserDto {
    data: UserDto;

    browserId: string;
    token: AuthenticationResponse;
}

export interface ProductDTO {
    data: ProductDTO;

    //id: number;
    productName: string;
    description: string;
    discount: boolean,
    // valueDiscount: number;
    price: number;
    stock: number;
    imageName: string;
}

export interface CartDto {
    data: any;

    productCount: number;
    sumAmount: number;  
    cartItems: CartItemsDto[];
}

export interface CartItemsDto{
    data: any;

    Id: number;
    price: number;
    count: number;
    productId: string,
    productName: string,
    image: string;
    colors: any;
}

export interface TreeViewCategoryDto {
    id: string;
    title: string;
    subs: Array<TreeViewCategoryDto>;
}

export interface CustomerDto {
    id: number;
    email: string;
    address: string;
    lastName: string;
    firstName: string;
    phoneNumber: string;
}

export interface AuthenticationResponse {
    token: string;
    expiration: Date;
}

export interface MessageDto{
    email: string;
    typeFeedBack: number;
    description: string;
}